using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Threads;

namespace Xlent.Lever.Libraries2.Core.Cache
{

    /// <summary>
    /// A <see cref="UseCacheStrategyDelegateAsync{TId}"/> should return one of these values.
    /// </summary>
    public enum UseCacheStrategyEnum
    {
        /// <summary>
        /// Use the cached value
        /// </summary>
        Use,
        /// <summary>
        /// Ignore the cached value, but keep it in the cache
        /// </summary>
        Ignore,
        /// <summary>
        /// Ignore the cached value, and remove it from the cache
        /// </summary>
        Remove
    }

    /// <summary>
    /// A delegate for flushing the cache, ie remove all items in the cache.
    /// </summary>
    public delegate Task FlushCacheDelegateAsync();

    /// <summary>
    /// The delegate should decide if we should even should try to get the data from the cache, or if we should go directly to the storage.
    /// </summary>
    public delegate Task<bool> UseCacheAtAllDelegateAsync(Type cachedItemType);

    /// <summary>
    /// The delegate will receive information about a cached item. Based on that information, the delegate should decide if the cached data should be deleted, ignored or used.
    /// Before the delegate is called, the cached item has already been vetted according to the <see cref="AutoCacheOptions"/>.
    /// This means for example that you will not have to check if the data is too old in the general sense.
    /// </summary>
    /// <param name="cachedItemInformation">Information about the cached item.</param>
    public delegate Task<UseCacheStrategyEnum> UseCacheStrategyDelegateAsync<TId>(CachedItemInformation<TId> cachedItemInformation);

    /// <summary>
    /// A delegate for getting a unique cache key from an item.
    /// </summary>
    /// <param name="item">The item to get the key for</param>
    public delegate TId GetIdDelegate<in TModel, out TId>(TModel item);

    /// <summary>
    /// Use this to put an "intelligent" cache between you and your ICrud storage.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class AutoCache<TModel, TId> : ICrud<TModel, TId>
    {
        private readonly IDistributedCache _cache;
        private readonly FlushCacheDelegateAsync _flushCacheDelegateAsync;
        private readonly GetIdDelegate<TModel, TId> _getIdDelegate;
        private readonly AutoCacheOptions _options;
        private readonly ICrud<TModel, TId> _storage;
        private readonly DistributedCacheEntryOptions _cacheOptions;
        private string _cacheIdentity;
        private const string ReadAllCacheKey = "ReadAllCacheKey";
        private readonly object _lockReadAllCache = new object();
        private readonly ConcurrentDictionary<string, PageEnvelope<TModel>> _activeCachingOfPages = new ConcurrentDictionary<string, PageEnvelope<TModel>>();

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
        public bool SaveReadAllToCacheThreadIsActive { get; private set; }

        /// <summary>
        /// Constructor for TModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public AutoCache(ICrud<TModel, TId> storage, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
        : this(storage, item => ((IUniquelyIdentifiable<TId>)item).Id, cache, flushCacheDelegateAsync, options)
        {
        }


        /// <summary>
        /// Constructor for TModel that does not implement <see cref="IUniquelyIdentifiable{TId}"/>, or when you want to specify your own GetKey() method.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="getIdDelegate"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public AutoCache(ICrud<TModel, TId> storage, GetIdDelegate<TModel, TId> getIdDelegate, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
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

            _cacheIdentity = Guid.NewGuid().ToString();
            _cache = cache;
            _flushCacheDelegateAsync = flushCacheDelegateAsync;
            _options = options;
            _storage = storage;
            _getIdDelegate = getIdDelegate;
            _cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = _options.SlidingExpiration
            };
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TModel item)
        {
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var createdItem = await _storage.CreateAndReturnAsync(item);
            await CacheSetAsync(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        public async Task<TId> CreateAsync(TModel item)
        {
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var id = await _storage.CreateAsync(item);
            await CacheMaybeSetAsync(id);
            return id;
        }

        /// <inheritdoc />
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId id, TModel item)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var createdItem = await _storage.CreateWithSpecifiedIdAndReturnAsync(id, item);
            await CacheSetAsync(id, createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(TId id, TModel item)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            await _storage.CreateWithSpecifiedIdAsync(id, item);
            await CacheMaybeSetAsync(id);
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync()
        {
            if (_flushCacheDelegateAsync == null)
            {
                var message = $"When deleting all items from the storage, a flush method for the cache was not specified for the model {typeof(TModel).FullName}, so we can't flush the items from the cache." +
                              "The items added before this will be ignored and there is no risk for inconsistency, but the cache keeps growing.";
                Log.LogWarning(message);
            }

            var task1 = _flushCacheDelegateAsync == null ? Task.CompletedTask : _flushCacheDelegateAsync();
            var task2 = _storage.DeleteAllAsync();
            await Task.WhenAll(task1, task2);
            _cacheIdentity = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var task1 = CacheRemoveAsync(id);
            var task2 = _storage.DeleteAsync(id);
            await Task.WhenAll(task1, task2);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> ReadAllAsync(int limit = 0)
        {
            var itemsArray = await CacheGetAsync();
            if (itemsArray != null) return itemsArray;
            var itemsCollection = await _storage.ReadAllAsync(limit);
            itemsArray = itemsCollection as TModel[] ?? itemsCollection.ToArray();
            lock (_lockReadAllCache)
            {
                if (SaveReadAllToCacheThreadIsActive) return itemsArray;
                SaveReadAllToCacheThreadIsActive = true;
            }
            ThreadHelper.FireAndForget(async () => await StoreAllItemsInCache(itemsArray).ConfigureAwait(false));
            return itemsArray;
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset = 0, int? limit = null)
        {
            if (limit == null) limit = PageInfo.DefaultLimit;
            var result = await CacheGetAsync(offset, limit.Value);
            if (result != null) return result;
            result = await _storage.ReadAllWithPagingAsync(offset, limit.Value);
            if (result?.Data == null) return null;
            ThreadHelper.FireAndForget(async () =>
                        await StoreAllItemsInCache(result, false).ConfigureAwait(false));
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

        /// <inheritdoc />
        public async Task<TModel> UpdateAndReturnAsync(TId id, TModel item)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var updatedItem = await _storage.UpdateAndReturnAsync(id, item);
            await CacheSetAsync(id, updatedItem);
            return updatedItem;

        }

        /// <inheritdoc />
        public async Task UpdateAsync(TId id, TModel item)
        {
            await _storage.UpdateAsync(id, item);
            await CacheMaybeSetAsync(id);
        }

        private async Task StoreAllItemsInCache(TModel[] itemsArray)
        {
            try
            {
                if (_options.SaveResultFromReadAll)
                {
                    // Maybe cache the entire array
                    var cacheArrayTask = CacheSetAsync(itemsArray);
                    var cachePageTasks = new List<Task>();

                    // Cache individual pages
                    var offset = 0;
                    while (offset < itemsArray.Length)
                    {
                        var data = itemsArray.Skip(offset).Take(PageInfo.DefaultLimit);
                        var pageEnvelope = new PageEnvelope<TModel>(offset, PageInfo.DefaultLimit, itemsArray.Length, data);
                        var task = StoreAllItemsInCache(pageEnvelope, true);
                        cachePageTasks.Add(task);
                        offset += PageInfo.DefaultLimit;
                    }
                    await Task.WhenAll(cachePageTasks);
                    await cacheArrayTask;
                }
                else
                {
                    // Cache individual items
                    var cacheIndividualItemTasks = itemsArray.Select(CacheSetAsync);
                    await Task.WhenAll(cacheIndividualItemTasks);
                }
            }
            finally
            {
                lock (_lockReadAllCache)
                {
                    SaveReadAllToCacheThreadIsActive = false;
                }
            }
        }

        private async Task StoreAllItemsInCache(PageEnvelope<TModel> pageEnvelope, bool inSaveReadAllThread)
        {
            // Give up if this is an individual page being saved while we are saving a total read all, 
            // because that could lead to inconsistencies in the data
            if (!inSaveReadAllThread && SaveReadAllToCacheThreadIsActive) return;

            var key = GetCacheKeyForPage(pageEnvelope.PageInfo.Offset, pageEnvelope.PageInfo.Limit);

            // Give up if a save of the same page is already active
            if (!_activeCachingOfPages.TryAdd(key, pageEnvelope)) return;

            try
            {
                var cacheIndividualItemTasks = pageEnvelope.Data.Select(CacheSetAsync);
                var cachePageTask = CacheSetAsync(pageEnvelope);
                await Task.WhenAll(cacheIndividualItemTasks);
                await cachePageTask;
            }
            finally
            {
                _activeCachingOfPages.TryRemove(key, out PageEnvelope<TModel> _);
            }
        }

        private async Task CacheMaybeSetAsync(TId id)
        {
            async Task<bool> IsAlreadyCachedAndGetIsOkToUpdate()
            {
                return _options.DoGetToUpdate && await CacheItemExistsAsync(id);
            }

            var getAndSave = _options.SaveAll || await IsAlreadyCachedAndGetIsOkToUpdate();
            if (!getAndSave) return;
            var item = await _storage.ReadAsync(id);
            await CacheSetAsync(id, item);
        }

        private async Task<bool> CacheItemExistsAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var key = GetCacheKeyFromId(id);
            var cachedItem = await _cache.GetAsync(key);
            return cachedItem != null;
        }

        private async Task<PageEnvelope<TModel>> CacheGetAsync(int offset, int limit)
        {
            if (UseCacheAtAllMethodAsync != null &&
                !await UseCacheAtAllMethodAsync(typeof(PageEnvelope<TModel>))) return null;
            var key = GetCacheKeyForPage(offset, limit);
            var byteArray = await _cache.GetAsync(key);
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
                    await _cache.RemoveAsync(key);
                    return null;
                default:
                    FulcrumAssert.Fail($"Unexpected value of {nameof(UseCacheStrategyEnum)} ({cacheItemStrategy}).");
                    return null;
            }
        }

        private async Task<TModel[]> CacheGetAsync()
        {
            if (UseCacheAtAllMethodAsync != null &&
                !await UseCacheAtAllMethodAsync(typeof(TModel[]))) return null;
            var byteArray = await _cache.GetAsync(ReadAllCacheKey);
            if (byteArray == null) return null;
            var cacheEnvelope = SupportMethods.Deserialize<CacheEnvelope>(byteArray);
            var cacheItemStrategy = GetCacheItemStrategy(cacheEnvelope);
            switch (cacheItemStrategy)
            {
                case UseCacheStrategyEnum.Use:
                    return SupportMethods.Deserialize<TModel[]>(cacheEnvelope.Data);
                case UseCacheStrategyEnum.Ignore:
                    return null;
                case UseCacheStrategyEnum.Remove:
                    await _cache.RemoveAsync(ReadAllCacheKey);
                    return null;
                default:
                    FulcrumAssert.Fail($"Unexpected value of {nameof(UseCacheStrategyEnum)} ({cacheItemStrategy}).");
                    return null;
            }
        }

        private async Task<TModel> CacheGetAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            if (UseCacheAtAllMethodAsync != null && !await UseCacheAtAllMethodAsync(typeof(TModel))) return default(TModel);
            var key = GetCacheKeyFromId(id);
            var byteArray = await _cache.GetAsync(key);
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
                    await CacheRemoveAsync(id);
                    return default(TModel);
                default:
                    FulcrumAssert.Fail($"Unexpected value of {nameof(UseCacheStrategyEnum)} ({cacheItemStrategy}).");
                    return default(TModel);
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

            if (cacheEnvelope.CacheIdentity != _cacheIdentity) return UseCacheStrategyEnum.Remove;
            return TooOld(cacheEnvelope) ? UseCacheStrategyEnum.Remove : UseCacheStrategyEnum.Use;
        }

        private bool TooOld(CacheEnvelope cacheEnvelope)
        {
            if (_options.AbsoluteExpirationRelativeToNow == null) return false;
            return cacheEnvelope.UpdatedAt.Add(_options.AbsoluteExpirationRelativeToNow.Value) <= DateTimeOffset.Now;
        }

        private Task CacheSetAsync(TModel item)
        {
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            return CacheSetAsync(_getIdDelegate(item), item);
        }

        private async Task CacheSetAsync(TId id, TModel item)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var key = GetCacheKeyFromId(id);
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(item);
            await _cache.SetAsync(key, serializedCacheEnvelope, _cacheOptions, CancellationToken.None);
        }

        private async Task CacheSetAsync(TModel[] itemsArray)
        {
            InternalContract.RequireNotDefaultValue(itemsArray, nameof(itemsArray));
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(itemsArray);
            await _cache.SetAsync(ReadAllCacheKey, serializedCacheEnvelope, _cacheOptions, CancellationToken.None);
        }

        private async Task CacheSetAsync(PageEnvelope<TModel> pageEnvelope)
        {
            InternalContract.RequireNotDefaultValue(pageEnvelope, nameof(pageEnvelope));
            InternalContract.RequireValidated(pageEnvelope, nameof(pageEnvelope));
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(pageEnvelope);
            var key = GetCacheKeyForPage(pageEnvelope.PageInfo.Offset, pageEnvelope.PageInfo.Limit);
            await _cache.SetAsync(key, serializedCacheEnvelope, _cacheOptions, CancellationToken.None);
        }

        private static string GetCacheKeyForPage(int offset, int limit)
        {
            return $"{ReadAllCacheKey}-{offset}-{limit}";
        }

        /// <summary>
        /// Serialize the <paramref name="item"/>, put it into an envelope and serialize the envelope
        /// </summary>
        public byte[] ToSerializedCacheEnvelope<T>(T item)
        {
            var serializedItem = SupportMethods.Serialize(item);
            var cacheEnvelope = new CacheEnvelope
            {
                CacheIdentity = _cacheIdentity,
                UpdatedAt = DateTimeOffset.Now,
                Data = serializedItem
            };
            var serializedCacheEnvelope = SupportMethods.Serialize(cacheEnvelope);
            return serializedCacheEnvelope;
        }

        /// <summary>
        ///Deserialize the <paramref name="serializedEnvelope"/> and deserialize the data in it.
        /// </summary>
        public T ToItem<T>(byte[] serializedEnvelope)
        {
            var cacheEnvelope = SupportMethods.Deserialize<CacheEnvelope>(serializedEnvelope);
            return SupportMethods.Deserialize<T>(cacheEnvelope.Data);
        }

        private async Task CacheRemoveAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var key = GetCacheKeyFromId(id);
            await _cache.RemoveAsync(key);
        }

        private static string GetCacheKeyFromId(TId id)
        {
            var key = id?.ToString();
            InternalContract.Require(key != null,
                $"Could not extract a cache key for an item of type {typeof(TModel).FullName}.");
            return key;
        }
    }
}
