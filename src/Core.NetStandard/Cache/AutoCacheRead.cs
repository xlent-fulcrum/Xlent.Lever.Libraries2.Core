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
        private readonly object _lockReadAllCache = new object();
        private int _limitOfItemsInReadAllCache;
        private readonly ConcurrentDictionary<string, PageEnvelope<TModel>> _activeCachingOfPages = new ConcurrentDictionary<string, PageEnvelope<TModel>>();
        private readonly ConcurrentDictionary<string, bool> _collectionOperations = new ConcurrentDictionary<string, bool>();
        protected readonly IDistributedCache Cache;
        protected readonly GetIdDelegate<TModel, TId> GetIdDelegate;
        protected readonly AutoCacheOptions Options;
        protected readonly DistributedCacheEntryOptions CacheOptions;
        protected string CacheIdentity { get; set; }
        private const string ReadAllCacheKey = "ReadAllCacheKey";


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
        public async Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset = 0, int? limit = null)
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

        protected void CacheItemsInBackground(TModel[] itemsArray, int limit, string key)
        {
            if (!_collectionOperations.TryAdd(key, true)) return;
            ThreadHelper.FireAndForget(async () =>
                await CacheItemCollectionOperationAsync(itemsArray, limit, key, true).ConfigureAwait(false));
        }

        protected void CacheItemsInBackground(PageEnvelope<TModel> pageEnvelope, int limit, string keyPrefix)
        {
            // Give up if this is an individual page being saved while we are operating on a larger scale, 
            // because that could lead to inconsistencies in the data
            if (IsCollectionOperationActive(keyPrefix)) return;

            var key = GetCacheKeyForPage(keyPrefix, pageEnvelope.PageInfo.Offset, pageEnvelope.PageInfo.Limit);
            // Give up if a save of the same page is already active
            if (!_activeCachingOfPages.TryAdd(key, pageEnvelope)) return;

            ThreadHelper.FireAndForget(async () =>
                await CacheItemPageOperationAsync(pageEnvelope, limit, keyPrefix, false, true).ConfigureAwait(false));
        }

        protected async Task RemoveCacheItemsInBackgroundAsync(string key, Func<Task<TModel[]>> getItemsToDelete)
        {
            if (!_collectionOperations.TryAdd(key, true)) return;
            ThreadHelper.FireAndForget(async () => await ReadAndDelete(key, getItemsToDelete));
        }

        private async Task ReadAndDelete(string key, Func<Task<TModel[]>> getItemsToDelete)
        {
            var itemsArray = await getItemsToDelete();
            if (itemsArray == null || itemsArray.Length == 0) return;
            await CacheItemCollectionOperationAsync(itemsArray, int.MaxValue, key, false).ConfigureAwait(false);
        }

        protected void RemoveCacheItemsInBackground(TModel[] itemsArray, string key)
        {
            if (!_collectionOperations.TryAdd(key, true)) return;
            ThreadHelper.FireAndForget(async () =>
                await CacheItemCollectionOperationAsync(itemsArray, int.MaxValue, key, false).ConfigureAwait(false));
        }

        protected void RemoveCacheItemsInBackground(PageEnvelope<TModel> pageEnvelope, int limit, string keyPrefix)
        {
            // Give up if this is an individual page being saved while we are operating on a larger scale, 
            // because that could lead to inconsistencies in the data
            if (IsCollectionOperationActive(keyPrefix)) return;

            var key = GetCacheKeyForPage(keyPrefix, pageEnvelope.PageInfo.Offset, pageEnvelope.PageInfo.Limit);
            // Give up if a save of the same page is already active
            if (!_activeCachingOfPages.TryAdd(key, pageEnvelope)) return;

            ThreadHelper.FireAndForget(async () =>
                await CacheItemPageOperationAsync(pageEnvelope, int.MaxValue, keyPrefix, false, false).ConfigureAwait(false));
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
                        var task = CacheItemPageOperationAsync(pageEnvelope, limit, key, true, isSetOperation);
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

        private async Task CacheItemPageOperationAsync(PageEnvelope<TModel> pageEnvelope, int limit, string keyPrefix, bool inCollectionOperation, bool isSetOperation)
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

        protected async Task<PageEnvelope<TModel>> CacheGetAsync(int offset, int limit, string keyPrefix)
        {
            if (UseCacheAtAllMethodAsync != null &&
                !await UseCacheAtAllMethodAsync(typeof(PageEnvelope<TModel>))) return null;
            var key = GetCacheKeyForPage(keyPrefix, offset, limit);
            var byteArray = await Cache.GetAsync(key);
            if (byteArray == null) return null;
            var cacheEnvelope = SupportMethods.Deserialize<CacheEnvelope>(byteArray);
            var cacheItemStrategy = GetCacheItemStrategy(cacheEnvelope);
            switch (cacheItemStrategy)
            {
                case UseCacheStrategyEnum.Use:
                    return SupportMethods.Deserialize<PageEnvelope<TModel>>(cacheEnvelope.Data);
                case UseCacheStrategyEnum.Ignore:
                    return null;
                case UseCacheStrategyEnum.Remove:
                    await Cache.RemoveAsync(key);
                    return null;
                default:
                    FulcrumAssert.Fail($"Unexpected value of {nameof(UseCacheStrategyEnum)} ({cacheItemStrategy}).");
                    return null;
            }
        }

        protected async Task<TModel[]> CacheGetAsync(int limit, string key)
        {
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            if (limit > _limitOfItemsInReadAllCache) return null;
            if (UseCacheAtAllMethodAsync != null &&
                !await UseCacheAtAllMethodAsync(typeof(TModel[]))) return null;
            var byteArray = await Cache.GetAsync(key);
            if (byteArray == null) return null;
            var cacheEnvelope = SupportMethods.Deserialize<CacheEnvelope>(byteArray);
            var cacheItemStrategy = GetCacheItemStrategy(cacheEnvelope);
            switch (cacheItemStrategy)
            {
                case UseCacheStrategyEnum.Use:
                    var array =  SupportMethods.Deserialize<TModel[]>(cacheEnvelope.Data);
                    if (limit > array.Length) return array;
                    var subset = array.Take(limit);
                    return subset as TModel[] ?? subset.ToArray();
                case UseCacheStrategyEnum.Ignore:
                    return null;
                case UseCacheStrategyEnum.Remove:
                    await Cache.RemoveAsync(ReadAllCacheKey);
                    return null;
                default:
                    FulcrumAssert.Fail($"Unexpected value of {nameof(UseCacheStrategyEnum)} ({cacheItemStrategy}).");
                    return null;
            }
        }

        protected async Task<TModel> CacheGetAsync(TId id, string key = null)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            if (UseCacheAtAllMethodAsync != null && !await UseCacheAtAllMethodAsync(typeof(TModel))) return default(TModel);
            key = key ?? GetCacheKeyFromId(id);
            var byteArray = await Cache.GetAsync(key);
            if (byteArray == null) return default(TModel);
            var cacheEnvelope = SupportMethods.Deserialize<CacheEnvelope>(byteArray);
            var cacheItemStrategy = await GetCacheItemStrategyAsync(id, cacheEnvelope);
            switch (cacheItemStrategy)
            {
                case UseCacheStrategyEnum.Use:
                    return SupportMethods.Deserialize<TModel>(cacheEnvelope.Data);
                case UseCacheStrategyEnum.Ignore:
                    return default(TModel);
                case UseCacheStrategyEnum.Remove:
                    await CacheRemoveByIdAsync(id);
                    return default(TModel);
                default:
                    FulcrumAssert.Fail($"Unexpected value of {nameof(UseCacheStrategyEnum)} ({cacheItemStrategy}).");
                    return default(TModel);
            }
        }

        protected async Task<UseCacheStrategyEnum> GetCacheItemStrategyAsync(TId id, CacheEnvelope cacheEnvelope)
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

        protected UseCacheStrategyEnum GetCacheItemStrategy(CacheEnvelope cacheEnvelope)
        {
            InternalContract.RequireNotNull(cacheEnvelope, nameof(cacheEnvelope));

            if (cacheEnvelope.CacheIdentity != CacheIdentity) return UseCacheStrategyEnum.Remove;
            return TooOld(cacheEnvelope) ? UseCacheStrategyEnum.Remove : UseCacheStrategyEnum.Use;
        }

        protected bool TooOld(CacheEnvelope cacheEnvelope)
        {
            if (Options.AbsoluteExpirationRelativeToNow == null) return false;
            return cacheEnvelope.UpdatedAt.Add(Options.AbsoluteExpirationRelativeToNow.Value) <= DateTimeOffset.Now;
        }

        protected Task CacheSetAsync(TModel item)
        {
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            return CacheSetAsync(GetIdDelegate(item), item);
        }

        protected async Task CacheSetAsync(TId id, TModel item, string key = null)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            key = key ?? GetCacheKeyFromId(id);
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(item);
            await Cache.SetAsync(key, serializedCacheEnvelope, CacheOptions, CancellationToken.None);
        }

        protected async Task CacheSetAsync(TModel[] itemsArray, int limit, string key)
        {
            InternalContract.RequireNotDefaultValue(itemsArray, nameof(itemsArray));
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(itemsArray);
            await Cache.SetAsync(key, serializedCacheEnvelope, CacheOptions, CancellationToken.None);
            _limitOfItemsInReadAllCache = limit;
        }

        protected async Task CacheSetAsync(PageEnvelope<TModel> pageEnvelope, string keyPrefix)
        {
            InternalContract.RequireNotDefaultValue(pageEnvelope, nameof(pageEnvelope));
            InternalContract.RequireValidated(pageEnvelope, nameof(pageEnvelope));
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(pageEnvelope);
            var key = GetCacheKeyForPage(keyPrefix, pageEnvelope.PageInfo.Offset, pageEnvelope.PageInfo.Limit);
            await Cache.SetAsync(key, serializedCacheEnvelope, CacheOptions, CancellationToken.None);
        }

        protected static string GetCacheKeyForPage(string prefix, int offset, int limit)
        {
            return $"{prefix}-{offset}-{limit}";
        }

        protected async Task CacheRemoveByIdAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var key = GetCacheKeyFromId(id);
            await Cache.RemoveAsync(key);
        }

        protected static string GetCacheKeyFromId(TId id)
        {
            var key = id?.ToString();
            InternalContract.Require(key != null,
                $"Could not extract a cache key for an item of type {typeof(TModel).FullName}.");
            return key;
        }

        /// <summary>
        /// Serialize the <paramref name="item"/>, put it into an envelope and serialize the envelope
        /// </summary>
        public byte[] ToSerializedCacheEnvelope<T>(T item)
        {
            var serializedItem = SupportMethods.Serialize(item);
            var cacheEnvelope = new CacheEnvelope
            {
                CacheIdentity = CacheIdentity,
                UpdatedAt = DateTimeOffset.Now,
                Data = serializedItem
            };
            var serializedCacheEnvelope = SupportMethods.Serialize(cacheEnvelope);
            return serializedCacheEnvelope;
        }
    }
}
