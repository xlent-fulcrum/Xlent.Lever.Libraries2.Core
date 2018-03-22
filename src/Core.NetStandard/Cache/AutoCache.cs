using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xlent.Lever.Libraries2.Core.Assert;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    /// <summary>
    /// Use this to put an "intelligent" cache between you and your ICrud storage.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class AutoCache<TModel, TId> : ICrud<TModel, TId>
    {
        private readonly IDistributedCache _cache;
        private readonly FlushCacheDelegateAsync _flushCacheDelegateAsync;
        private readonly GetIdDelegate _getIdDelegate;
        private readonly AutoCacheOptions _options;
        private readonly ICrud<TModel, TId> _storage;
        private readonly DistributedCacheEntryOptions _cacheOptions;
        private string _cacheIdentity;

        /// <summary>
        /// A delegate for getting a unique cache key from an item.
        /// </summary>
        /// <param name="item">The item to get the key for</param>
        public delegate TId GetIdDelegate(TModel item);

        /// <summary>
        /// A delegate for flushing the cache, ie remove all items in the cache.
        /// </summary>
        public delegate Task<TId> FlushCacheDelegateAsync();

        /// <summary>
        /// A <see cref="AutoCache{TModel,TId}.UseCacheStrategyDelegateAsync"/> should return one of these values.
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
        /// The delegate will receive information about a cached item. Based on that information, the delegate should decide if the cached data should be deleted, ignored or used.
        /// Before the delegate is called, the cached item has already been vetted according to the <see cref="AutoCacheOptions"/>.
        /// This means for example that you will not have to check if the data is too old in the general sense.
        /// </summary>
        /// <param name="cachedItemInformation">Information about the cached item.</param>
        public delegate Task<UseCacheStrategyEnum> UseCacheStrategyDelegateAsync(CachedItemInformation<TId> cachedItemInformation);

        /// <summary>
        /// The delegate should decide if we should even should try to get the data from the cache, or if we should go directly to the storage.
        /// </summary>
        public delegate Task<bool> UseCacheAtAllDelegateAsync(Type cachedItemType);

        /// <summary>
        /// If you want to have your own method for discarding cache values, set this property to a method that returns how you want to deal with the cached value.
        /// </summary>
        public UseCacheStrategyDelegateAsync UseCacheStrategyMethodAsync { get; set; }

        /// <summary>
        /// If you want to be able to sometimes totally ignore the cache (saving yourself from a call to the cache), use this method to decide what to do.
        /// </summary>
        public UseCacheAtAllDelegateAsync UseCacheAtAllMethodAsync { get; set; }

        /// <summary>
        /// Constructor for TModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public AutoCache(ICrud<TModel, TId> storage, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
        :this(storage, item => ((IUniquelyIdentifiable<TId>)item).Id, cache, flushCacheDelegateAsync, options)
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
        public AutoCache(ICrud<TModel, TId> storage, GetIdDelegate getIdDelegate, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
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
            var createdItem = await _storage.CreateAndReturnAsync(item);
            await CacheSetAsync(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        public async Task<TId> CreateAsync(TModel item)
        {
            var id = await _storage.CreateAsync(item);
            await CacheMaybeSetAsync(id);
            return id;
        }

        /// <inheritdoc />
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId id, TModel item)
        {
            var createdItem = await _storage.CreateWithSpecifiedIdAndReturnAsync(id, item);
            await CacheSetAsync(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(TId id, TModel item)
        {
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
            var task1 = CacheRemoveAsync(id);
            var task2 = _storage.DeleteAsync(id);
            await Task.WhenAll(task1, task2);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> ReadAllAsync(int limit = 0)
        {
            var itemsCollection = await _storage.ReadAllAsync(limit);
            var itemsArray = itemsCollection as TModel[] ?? itemsCollection.ToArray();
            var tasks = itemsArray.Select(CacheSetAsync);
            await Task.WhenAll(tasks);
            return itemsArray;
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset = 0, int? limit = null)
        {
            var result = await _storage.ReadAllWithPagingAsync(offset, limit);
            if (result?.Data == null) return null;
            var tasks = result.Data.Select(CacheSetAsync);
            await Task.WhenAll(tasks);
            return result;
        }

        /// <inheritdoc />
        public async Task<TModel> ReadAsync(TId id)
        {
            var item = await CacheGetAsync(id);
            if (item != null) return item;
            item = await _storage.ReadAsync(id);
            await CacheSetAsync(item);
            return item;
        }

        /// <inheritdoc />
        public async Task<TModel> UpdateAndReturnAsync(TId id, TModel item)
        {
            var updatedItem = await _storage.UpdateAndReturnAsync(id, item);
            await CacheSetAsync(updatedItem);
            return updatedItem;

        }

        /// <inheritdoc />
        public async Task UpdateAsync(TId id, TModel item)
        {
            await _storage.UpdateAsync(id, item);
            await CacheMaybeSetAsync(id);
        }

        private async Task CacheMaybeSetAsync(TId id)
        {
            var getAndSave = _options.SaveAll || _options.GetToUpdate && await CacheItemExistsAsync(id);
            if (!getAndSave) return;
            var item = await _storage.ReadAsync(id);
            await CacheSetAsync(item);
        }

        private async Task<bool> CacheItemExistsAsync(TId id)
        {
            InternalContract.RequireNotNull(id, nameof(id));
            var key = id.ToString();
            var cachedItem = await _cache.GetAsync(key);
            return cachedItem != null;
        }

        private async Task<TModel> CacheGetAsync(TId id)
        {
            InternalContract.RequireNotNull(id, nameof(id));
            if (UseCacheAtAllMethodAsync != null && !await UseCacheAtAllMethodAsync(typeof(TModel))) return default(TModel);
            var key = GetKeyFromId(id);
            var byteArray = await _cache.GetAsync(key);
            if (byteArray == null) return default(TModel);
            var cacheEnvelope = Deserialize<CacheEnvelope>(byteArray);
            var cacheItemStrategy = await GetCacheItemStrategyAsync(id, cacheEnvelope);
            switch (cacheItemStrategy)
            {
                case UseCacheStrategyEnum.Use:
                    return Deserialize<TModel>(cacheEnvelope.Data);
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
            InternalContract.RequireNotNull(id, nameof(id));
            InternalContract.RequireNotNull(cacheEnvelope, nameof(cacheEnvelope));

            if (cacheEnvelope.CacheIdentity != _cacheIdentity) return UseCacheStrategyEnum.Remove;
            if (TooOld(cacheEnvelope)) return UseCacheStrategyEnum.Remove;
            if (UseCacheStrategyMethodAsync != null)
            {
                var cachedItemInformation = new CachedItemInformation<TId>
                {
                    Id = id,
                    UpdatedAt = cacheEnvelope.UpdatedAt
                };
                return await UseCacheStrategyMethodAsync(cachedItemInformation);
            }
            return UseCacheStrategyEnum.Use;
        }

        private bool TooOld(CacheEnvelope cacheEnvelope)
        {
            return cacheEnvelope.UpdatedAt.Add(_options.AbsoluteExpirationRelativeToNow) <= DateTimeOffset.Now;
        }

        private async Task CacheSetAsync(TModel item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            var key = GetKeyFromId(_getIdDelegate(item));
            var serializedCacheEnvelope = ToSerializedCacheEnvelope(item);
            await _cache.SetAsync(key, serializedCacheEnvelope, _cacheOptions, CancellationToken.None);
        }

        /// <summary>
        /// Serialize the <paramref name="item"/>, put it into an envelope and serialize the envelope
        /// </summary>
        public byte[] ToSerializedCacheEnvelope(TModel item)
        {
            var serializedItem = Serialize(item);
            var cacheEnvelope = new CacheEnvelope
            {
                CacheIdentity = _cacheIdentity,
                UpdatedAt = DateTimeOffset.Now,
                Data = serializedItem
            };
            var serializedCacheEnvelope = Serialize(cacheEnvelope);
            return serializedCacheEnvelope;
        }

        private async Task CacheRemoveAsync(TId id)
        {
            InternalContract.RequireNotNull(id, nameof(id));
            var key = GetKeyFromId(id);
            await _cache.RemoveAsync(key);
        }

        private static string GetKeyFromId(TId id)
        {
            var key = id?.ToString();
            InternalContract.Require(key != null,
                $"Could not extract a cache key for an item of type {typeof(TModel).FullName}.");
            return key;
        }

        private static byte[] Serialize<T>(T item)
        {
            var itemAsJsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(itemAsJsonString);
        }

        private static T Deserialize<T>(byte[] itemAsBytes)
        {
            var itemAsJsonString = Encoding.UTF8.GetString(itemAsBytes);
            return JsonConvert.DeserializeObject<T>(itemAsJsonString);
        }
    }
}
