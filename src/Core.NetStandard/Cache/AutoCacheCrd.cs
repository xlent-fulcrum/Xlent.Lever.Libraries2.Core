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
    public class AutoCacheCrd<TModel, TId> : AutoCacheRead<TModel, TId>, ICrd<TModel, TId>
    {
        private readonly ICrd<TModel, TId> _storage;
        protected readonly FlushCacheDelegateAsync FlushCacheDelegateAsync;
        /// <summary>
        /// Constructor for TModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public AutoCacheCrd(ICrd<TModel, TId> storage, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
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
        public AutoCacheCrd(ICrd<TModel, TId> storage, GetIdDelegate<TModel, TId> getIdDelegate, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
        :base(storage, getIdDelegate, cache, options)
        {
            _storage = storage;
            FlushCacheDelegateAsync = flushCacheDelegateAsync;
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
            if (FlushCacheDelegateAsync == null)
            {
                var message = $"When deleting all items from the storage, a flush method for the cache was not specified for the model {typeof(TModel).FullName}, so we can't flush the items from the cache." +
                              "The items added before this will be ignored and there is no risk for inconsistency, but the cache keeps growing.";
                Log.LogWarning(message);
            }

            var task1 = FlushCacheDelegateAsync == null ? Task.CompletedTask : FlushCacheDelegateAsync();
            var task2 = _storage.DeleteAllAsync();
            await Task.WhenAll(task1, task2);
            CacheIdentity = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var task1 = CacheRemoveAsync(id);
            var task2 = _storage.DeleteAsync(id);
            await Task.WhenAll(task1, task2);
        }

        protected async Task CacheMaybeSetAsync(TId id)
        {
            async Task<bool> IsAlreadyCachedAndGetIsOkToUpdate()
            {
                return Options.DoGetToUpdate && await CacheItemExistsAsync(id);
            }

            var getAndSave = Options.SaveAll || await IsAlreadyCachedAndGetIsOkToUpdate();
            if (!getAndSave) return;
            var item = await _storage.ReadAsync(id);
            await CacheSetAsync(id, item);
        }

        protected async Task<bool> CacheItemExistsAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var key = GetCacheKeyFromId(id);
            var cachedItem = await Cache.GetAsync(key);
            return cachedItem != null;
        }
    }
}
