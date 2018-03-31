using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Threads;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    /// <summary>
    /// Use this to put an "intelligent" cache between you and your ICrud storage.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class AutoCacheRead<TModel, TId> : IReadAll<TModel, TId>
    {
        private readonly IReadAll<TModel, TId> _storage;
        private int _limitOfItemsInReadAllCache;
        private readonly ConcurrentDictionary<string, PageEnvelope<TModel>> _activeCachingOfPages = new ConcurrentDictionary<string, PageEnvelope<TModel>>();
        private readonly ConcurrentDictionary<string, bool> _collectionOperations = new ConcurrentDictionary<string, bool>();
        private const string ReadAllCacheKey = "ReadAllCacheKey";

        /// <summary>
        /// The cache of items
        /// </summary>
        protected readonly IDistributedCache Cache;

        /// <summary>
        /// A method for getting the id of an item.
        /// </summary>
        protected GetIdDelegate<TModel, TId> GetIdDelegate { get; }

        /// <summary>
        /// The options for the AutoCache
        /// </summary>
        protected AutoCacheOptions Options { get; }

        /// <summary>
        /// The options we should use when creating new items in the Distributed cache.
        /// </summary>
        protected  DistributedCacheEntryOptions CacheOptions { get; }

        /// <summary>
        /// The identity of the current cache. For caches that doesn't support flushing, this is how we forget about all old values.
        /// </summary>
        protected string CacheIdentity { get; set; }


        /// <summary>
        /// If you want to have your own method for discarding cache values, set this property to a method that returns how you want to deal with the cached value.
        /// </summary>
        public UseCacheStrategyDelegateAsync<TId> UseCacheStrategyMethodAsync { get; set; }

        /// <summary>
        /// If you want to be able to sometimes totally ignore the cache (saving yourself from a call to the cache), use this method to decide what to do.
        /// </summary>
        public UseCacheAtAllDelegateAsync UseCacheAtAllMethodAsync { get; set; }

        /// <summary>
        /// True while a background thread is active saving results from a ReadAll() operation.
        /// </summary>
        protected bool IsCollectionOperationActive(string key)
        {
            return _collectionOperations.ContainsKey(key);
        }

        /// <summary>
        /// True while a background thread is active saving results from a ReadAll() operation.
        /// </summary>
        public bool IsCollectionOperationActive()
        {
            return IsCollectionOperationActive(ReadAllCacheKey);
        }

        /// <summary>
        /// Constructor for TModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="options"></param>
        public AutoCacheRead(IReadAll<TModel, TId> storage, IDistributedCache cache, AutoCacheOptions options = null)
        : this(storage, item => ((IUniquelyIdentifiable<TId>)item).Id, cache, options)
        {
        }


        /// <summary>
        /// Constructor for TModel that does not implement <see cref="IUniquelyIdentifiable{TId}"/>, or when you want to specify your own GetKey() method.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="getIdDelegate"></param>
        /// <param name="options"></param>
        public AutoCacheRead(IReadAll<TModel, TId> storage, GetIdDelegate<TModel, TId> getIdDelegate, IDistributedCache cache, AutoCacheOptions options = null)
        {
            InternalContract.RequireNotNull(storage, nameof(storage));
            InternalContract.RequireNotNull(getIdDelegate, nameof(getIdDelegate));
            InternalContract.RequireNotNull(cache, nameof(cache));
            if (options == null)
            {
                options = new AutoCacheOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };
            }

            CacheIdentity = Guid.NewGuid().ToString();
            Cache = cache;
            Options = options;
            _storage = storage;
            GetIdDelegate = getIdDelegate;
            CacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = Options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = Options.SlidingExpiration
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> ReadAllAsync(int limit = int.MaxValue)
        {
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            if (limit == 0) limit = int.MaxValue;
            var itemsArray = await CacheGetAsync(limit, ReadAllCacheKey);
            if (itemsArray != null) return itemsArray;
            var itemsCollection = await _storage.ReadAllAsync(limit);
            itemsArray = itemsCollection as TModel[] ?? itemsCollection.ToArray();
            CacheItemsInBackground(itemsArray, limit, ReadAllCacheKey);
            return itemsArray;
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset, int? limit = null)
        {
            if (limit == null) limit = PageInfo.DefaultLimit;
            var result = await CacheGetAsync(offset, limit.Value, ReadAllCacheKey);
            if (result != null) return result;
            result = await _storage.ReadAllWithPagingAsync(offset, limit.Value);
            if (result?.Data == null) return null;
            CacheItemsInBackground(result, limit.Value, ReadAllCacheKey);
            return result;
        }

        /// <inheritdoc />
        public async Task<TModel> ReadAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var item = await CacheGetAsync(id);
            if (item != null) return item;
            item = await _storage.ReadAsync(id);
            await CacheSetAsync(id, item);
            return item;
        }

        /// <summary>
        /// Add the items to the cache as a background process.
        /// </summary>
        protected internal void CacheItemsInBackground(TModel[] itemsArray, int limit, string key)
        {
            if (!_collectionOperations.TryAdd(key, true)) return;
            ThreadHelper.FireAndForget(async () =>
                await CacheItemCollectionOperationAsync(itemsArray, limit, key, true).ConfigureAwait(false));
        }

        /// <summary>
        /// Add the items to the cache as a background process.
        /// </summary>
        protected internal void CacheItemsInBackground(PageEnvelope<TModel> pageEnvelope, int limit, string keyPrefix)
        {
            // Give up if this is an individual page being saved while we are operating on a larger scale, 
            // because that could lead to inconsistencies in the data
            if (IsCollectionOperationActive(keyPrefix)) return;

            var key = GetCacheKeyForPage(keyPrefix, pageEnvelope.PageInfo.Offset, pageEnvelope.PageInfo.Limit);
            // Give up if a save of the same page is already active
            if (!_activeCachingOfPages.TryAdd(key, pageEnvelope)) return;

            ThreadHelper.FireAndForget(async () =>
                await CacheItemPageOperationAsync(pageEnvelope, limit, keyPrefix, true).ConfigureAwait(false));
        }

        /// <summary>
        /// Remove items from the cache as a background process.
        /// </summary>
        /// <param name="key">The key into the cache.</param>
        /// <param name="getItemsToDelete">A method that gets the items that should be deleted.</param>
        protected async Task RemoveCacheItemsInBackgroundAsync(string key, Func<Task<TModel[]>> getItemsToDelete)
        {
            if (!_collectionOperations.TryAdd(key, true)) return;
            ThreadHelper.FireAndForget(async () => await ReadAndDelete(key, getItemsToDelete));
            await Task.Yield();
        }

        private async Task ReadAndDelete(string key, Func<Task<TModel[]>> getItemsToDelete)
        {
            var itemsArray = await getItemsToDelete();
            if (itemsArray == null || itemsArray.Length == 0) return;
            await CacheItemCollectionOperationAsync(itemsArray, int.MaxValue, key, false).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove items from the cache as a background process.
        /// </summary>
        /// <param name="key">The key into the cache.</param>
        /// <param name="itemsArray">An array of items that should be removed from the cache.</param>
        protected void RemoveCacheItemsInBackground(TModel[] itemsArray, string key)
        {
            if (!_collectionOperations.TryAdd(key, true)) return;
            ThreadHelper.FireAndForget(async () =>
                await CacheItemCollectionOperationAsync(itemsArray, int.MaxValue, key, false).ConfigureAwait(false));
        }

        /// <summary>
        /// Remove items from the cache as a background process.
        /// </summary>
        /// <param name="pageEnvelope">A page with the items to delete.</param>
        /// <param name="keyPrefix">The key into the cache.</param>
        /// 
        protected void RemoveCacheItemsInBackground(PageEnvelope<TModel> pageEnvelope, string keyPrefix)
        {
            // Give up if this is an individual page being saved while we are operating on a larger scale, 
            // because that could lead to inconsistencies in the data
            if (IsCollectionOperationActive(keyPrefix)) return;

            var key = GetCacheKeyForPage(keyPrefix, pageEnvelope.PageInfo.Offset, pageEnvelope.PageInfo.Limit);
            // Give up if a save of the same page is already active
            if (!_activeCachingOfPages.TryAdd(key, pageEnvelope)) return;

            ThreadHelper.FireAndForget(async () =>
                await CacheItemPageOperationAsync(pageEnvelope, int.MaxValue, keyPrefix, false).ConfigureAwait(false));
        }

        private async Task CacheItemCollectionOperationAsync(TModel[] itemsArray, int limit, string key, bool isSetOperation)
        {
            InternalContract.RequireNotNull(itemsArray, nameof(itemsArray));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            if (!isSetOperation)
            {
                InternalContract.Require(limit == int.MaxValue, $"When {nameof(isSetOperation)} is false, then {nameof(limit)} must be set to int.MaxValue ({int.MaxValue}), but it was set to {limit}.");
            }
            try
            {
                if (Options.SaveCollections)
                {

                    var cacheArrayTask = isSetOperation 
                        ? CacheSetAsync(itemsArray, limit, key)
                        : Cache.RemoveAsync(key);
                    var cachePageTasks = new List<Task>();

                    // Cache individual pages
                    var offset = 0;
                    while (offset < itemsArray.Length)
                    {
                        var data = itemsArray.Skip(offset).Take(PageInfo.DefaultLimit);
                        var pageEnvelope = new PageEnvelope<TModel>(offset, PageInfo.DefaultLimit, itemsArray.Length, data);
                        var task = CacheItemPageOperationAsync(pageEnvelope, limit, key, isSetOperation);
                        cachePageTasks.Add(task);
                        offset += PageInfo.DefaultLimit;
                    }
                    await Task.WhenAll(cachePageTasks);
                    await cacheArrayTask;
                }
                else
                {
                    // Cache individual items
                    var cacheIndividualItemTasks = isSetOperation 
                    ? itemsArray.Select(CacheSetAsync)
                        : itemsArray.Select(item => CacheRemoveByIdAsync(GetIdDelegate(item)));
                    await Task.WhenAll(cacheIndividualItemTasks);
                }
            }
            finally
            {
                _collectionOperations.TryRemove(key, out _);
            }
        }

        private async Task CacheItemPageOperationAsync(PageEnvelope<TModel> pageEnvelope, int limit, string keyPrefix, bool isSetOperation)
        {
            var key = GetCacheKeyForPage(keyPrefix, pageEnvelope.PageInfo.Offset, pageEnvelope.PageInfo.Limit);
            try
            {
                var cacheIndividualItemTasks = isSetOperation
                    ? pageEnvelope.Data.Select(CacheSetAsync)
                    : pageEnvelope.Data.Select(item => CacheRemoveByIdAsync(GetIdDelegate(item)));
                if (!PageWasTruncated(pageEnvelope.PageInfo, limit))
                {
                    if (isSetOperation) await CacheSetAsync(pageEnvelope, keyPrefix);
                    else await Cache.RemoveAsync(key);
                }
                await Task.WhenAll(cacheIndividualItemTasks);
            }
            finally
            {
                _activeCachingOfPages.TryRemove(key, out PageEnvelope<TModel> _);
            }
        }

        private bool PageWasTruncated(PageInfo pageInfo, int limit)
        {
            if (pageInfo.Total != null && limit > pageInfo.Total.Value) return false;
            var count = pageInfo.Offset * pageInfo.Limit + pageInfo.Returned;
            return count == limit;
        }

        /// <summary>
        /// Get a value from the cache if all constraints are fulfilled.
        /// </summary>
        protected internal async Task<PageEnvelope<TModel>> CacheGetAsync(int offset, int limit, string keyPrefix)
        {
            if (UseCacheAtAllMethodAsync != null &&
                !await UseCacheAtAllMethodAsync(typeof(PageEnvelope<TModel>))) return null;
            var key = GetCacheKeyForPage(keyPrefix, offset, limit);
            return await GetAndMaybeReturnAsync<PageEnvelope<TModel>>(key, cacheEnvelope => SerializingSupport.Deserialize<PageEnvelope<TModel>>(cacheEnvelope.Data));
        }

        /// <summary>
        /// Get a value from the cache if all constraints are fulfilled.
        /// </summary>
        protected internal async Task<TModel[]> CacheGetAsync(int limit, string key)
        {
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            if (limit > _limitOfItemsInReadAllCache) return null;
            if (UseCacheAtAllMethodAsync != null &&
                !await UseCacheAtAllMethodAsync(typeof(TModel[]))) return null;

            return await GetAndMaybeReturnAsync<TModel[]>(key, cacheEnvelope =>
                {
                    var array = SerializingSupport.Deserialize<TModel[]>(cacheEnvelope.Data);
                    if (limit > array.Length) return array;
                    var subset = array.Take(limit);
                    return subset as TModel[] ?? subset.ToArray();
                });
        }

        /// <summary>
        /// Get a value from the cache if all constraints are fulfilled.
        /// </summary>
        private async Task<TModel> CacheGetAsync(TId id, string key = null)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            if (UseCacheAtAllMethodAsync != null && !await UseCacheAtAllMethodAsync(typeof(TModel))) return default(TModel);
            key = key ?? GetCacheKeyFromId(id);

            return await GetAndMaybeReturnAsync<TModel>(key, cacheEnvelope => SerializingSupport.Deserialize<TModel>(cacheEnvelope.Data), id);
        }

        private delegate TReturn DeserializeDelegate<out TReturn>(CacheEnvelope cacheEnvelope);

        private async Task<TReturn> GetAndMaybeReturnAsync<TReturn>(string key, DeserializeDelegate<TReturn> deserializeDelegate, TId id = default(TId))
        {
            var byteArray = await Cache.GetAsync(key);
            if (byteArray == null) return default(TReturn);
            var cacheEnvelope = SerializingSupport.Deserialize<CacheEnvelope>(byteArray);
            var cacheItemStrategy = Equals(id, default(TId)) ? GetCacheItemStrategy(cacheEnvelope) : await GetCacheItemStrategyAsync(id, cacheEnvelope);
            switch (cacheItemStrategy)
            {
                case UseCacheStrategyEnum.Use:
                    return deserializeDelegate(cacheEnvelope);
                case UseCacheStrategyEnum.Ignore:
                    return default(TReturn);
                case UseCacheStrategyEnum.Remove:
                    await Cache.RemoveAsync(key);
                    return default(TReturn);
                default:
                    FulcrumAssert.Fail($"Unexpected value of {nameof(UseCacheStrategyEnum)} ({cacheItemStrategy}).");
                    return default(TReturn);
            }
        }

        private async Task<UseCacheStrategyEnum> GetCacheItemStrategyAsync(TId id, CacheEnvelope cacheEnvelope)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotNull(cacheEnvelope, nameof(cacheEnvelope));

            var cacheItemStrategy = GetCacheItemStrategy(cacheEnvelope);
            if (cacheItemStrategy != UseCacheStrategyEnum.Use) return cacheItemStrategy;
            if (UseCacheStrategyMethodAsync == null) return UseCacheStrategyEnum.Use;
            var cachedItemInformation = new CachedItemInformation<TId>
            {
                Id = id,
                UpdatedAt = cacheEnvelope.UpdatedAt
            };
            return await UseCacheStrategyMethodAsync(cachedItemInformation);
        }

        private UseCacheStrategyEnum GetCacheItemStrategy(CacheEnvelope cacheEnvelope)
        {
            InternalContract.RequireNotNull(cacheEnvelope, nameof(cacheEnvelope));

            if (cacheEnvelope.CacheIdentity != CacheIdentity) return UseCacheStrategyEnum.Remove;
            return TooOld(cacheEnvelope) ? UseCacheStrategyEnum.Remove : UseCacheStrategyEnum.Use;
        }

        private bool TooOld(CacheEnvelope cacheEnvelope)
        {
            if (Options.AbsoluteExpirationRelativeToNow == null) return false;
            return cacheEnvelope.UpdatedAt.Add(Options.AbsoluteExpirationRelativeToNow.Value) <= DateTimeOffset.Now;
        }

        /// <summary>
        /// Put the item in the cache.
        /// </summary>
        protected Task CacheSetAsync(TModel item)
        {
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            return CacheSetAsync(GetIdDelegate(item), item);
        }

        /// <summary>
        /// Put the item in the cache.
        /// </summary>
        protected async Task CacheSetAsync(TId id, TModel item, string key = null)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            key = key ?? GetCacheKeyFromId(id);
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(item);
            await Cache.SetAsync(key, serializedCacheEnvelope, CacheOptions, CancellationToken.None);
        }

        /// <summary>
        /// Put the item in the cache.
        /// </summary>
        private async Task CacheSetAsync(TModel[] itemsArray, int limit, string key)
        {
            InternalContract.RequireNotDefaultValue(itemsArray, nameof(itemsArray));
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(itemsArray);
            await Cache.SetAsync(key, serializedCacheEnvelope, CacheOptions, CancellationToken.None);
            _limitOfItemsInReadAllCache = limit;
        }

        /// <summary>
        /// Put the item in the cache.
        /// </summary>
        private async Task CacheSetAsync(PageEnvelope<TModel> pageEnvelope, string keyPrefix)
        {
            InternalContract.RequireNotDefaultValue(pageEnvelope, nameof(pageEnvelope));
            InternalContract.RequireValidated(pageEnvelope, nameof(pageEnvelope));
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(pageEnvelope);
            var key = GetCacheKeyForPage(keyPrefix, pageEnvelope.PageInfo.Offset, pageEnvelope.PageInfo.Limit);
            await Cache.SetAsync(key, serializedCacheEnvelope, CacheOptions, CancellationToken.None);
        }

        /// <summary>
        /// Get the cache key for a specific page
        /// </summary>
        protected static string GetCacheKeyForPage(string prefix, int offset, int limit)
        {
            return $"{prefix}-{offset}-{limit}";
        }

        /// <summary>
        /// Get the cacke key
        /// </summary>
        protected static string GetCacheKeyFromId(TId id)
        {
            var key = id?.ToString();
            InternalContract.Require(key != null,
                $"Could not extract a cache key for an item of type {typeof(TModel).FullName}.");
            return key;
        }

        /// <summary>
        /// Remove from cache
        /// </summary>
        protected async Task CacheRemoveByIdAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var key = GetCacheKeyFromId(id);
            await Cache.RemoveAsync(key);
        }

        /// <summary>
        /// Serialize the <paramref name="item"/>, put it into an envelope and serialize the envelope
        /// </summary>
        public byte[] ToSerializedCacheEnvelope<T>(T item)
        {
            var serializedItem = SerializingSupport.Serialize(item);
            var cacheEnvelope = new CacheEnvelope
            {
                CacheIdentity = CacheIdentity,
                UpdatedAt = DateTimeOffset.Now,
                Data = serializedItem
            };
            var serializedCacheEnvelope = SerializingSupport.Serialize(cacheEnvelope);
            return serializedCacheEnvelope;
        }
    }
}
