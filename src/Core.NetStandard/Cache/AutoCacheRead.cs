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
    /// Use this to put an "intelligent" cache between you and your ICrud storage.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class AutoCacheRead<TModel, TId> : IReadAll<TModel, TId>
    {
        private readonly IReadAll<TModel, TId> _storage;
        private readonly object _lockReadAllCache = new object();
        private readonly ConcurrentDictionary<string, PageEnvelope<TModel>> _activeCachingOfPages = new ConcurrentDictionary<string, PageEnvelope<TModel>>();
        protected readonly IDistributedCache Cache;
        protected readonly GetIdDelegate<TModel, TId> GetIdDelegate;
        protected readonly AutoCacheOptions Options;
        protected readonly DistributedCacheEntryOptions CacheOptions;
        protected string CacheIdentity { get; set; }
        protected const string ReadAllCacheKey = "ReadAllCacheKey";


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
        public bool SaveReadAllToCacheThreadIsActive { get; protected set; }

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

        protected async Task StoreAllItemsInCache(TModel[] itemsArray)
        {
            try
            {
                if (Options.SaveResultFromReadAll)
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

        protected async Task StoreAllItemsInCache(PageEnvelope<TModel> pageEnvelope, bool inSaveReadAllThread)
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

        protected async Task<PageEnvelope<TModel>> CacheGetAsync(int offset, int limit)
        {
            if (UseCacheAtAllMethodAsync != null &&
                !await UseCacheAtAllMethodAsync(typeof(PageEnvelope<TModel>))) return null;
            var key = GetCacheKeyForPage(offset, limit);
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

        protected async Task<TModel[]> CacheGetAsync()
        {
            if (UseCacheAtAllMethodAsync != null &&
                !await UseCacheAtAllMethodAsync(typeof(TModel[]))) return null;
            var byteArray = await Cache.GetAsync(ReadAllCacheKey);
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
                    await Cache.RemoveAsync(ReadAllCacheKey);
                    return null;
                default:
                    FulcrumAssert.Fail($"Unexpected value of {nameof(UseCacheStrategyEnum)} ({cacheItemStrategy}).");
                    return null;
            }
        }

        protected async Task<TModel> CacheGetAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            if (UseCacheAtAllMethodAsync != null && !await UseCacheAtAllMethodAsync(typeof(TModel))) return default(TModel);
            var key = GetCacheKeyFromId(id);
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
                    await CacheRemoveAsync(id);
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

        protected async Task CacheSetAsync(TId id, TModel item)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var key = GetCacheKeyFromId(id);
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(item);
            await Cache.SetAsync(key, serializedCacheEnvelope, CacheOptions, CancellationToken.None);
        }

        protected async Task CacheSetAsync(TModel[] itemsArray)
        {
            InternalContract.RequireNotDefaultValue(itemsArray, nameof(itemsArray));
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(itemsArray);
            await Cache.SetAsync(ReadAllCacheKey, serializedCacheEnvelope, CacheOptions, CancellationToken.None);
        }

        protected async Task CacheSetAsync(PageEnvelope<TModel> pageEnvelope)
        {
            InternalContract.RequireNotDefaultValue(pageEnvelope, nameof(pageEnvelope));
            InternalContract.RequireValidated(pageEnvelope, nameof(pageEnvelope));
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(pageEnvelope);
            var key = GetCacheKeyForPage(pageEnvelope.PageInfo.Offset, pageEnvelope.PageInfo.Limit);
            await Cache.SetAsync(key, serializedCacheEnvelope, CacheOptions, CancellationToken.None);
        }

        protected static string GetCacheKeyForPage(int offset, int limit)
        {
            return $"{ReadAllCacheKey}-{offset}-{limit}";
        }

        protected async Task CacheRemoveAsync(TId id)
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
