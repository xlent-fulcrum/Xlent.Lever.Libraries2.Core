using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Cache
{
    /// <summary>
    /// Use this to put an "intelligent" cache between you and your ICrud storage.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class CrdAutoCache<TModel, TId> : ReadAutoCache<TModel, TId>, ICrd<TModel, TId>
    {
        private readonly ICrd<TModel, TId> _storage;

        /// <summary>
        /// Constructor for TModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public CrdAutoCache(ICrd<TModel, TId> storage, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
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
        public CrdAutoCache(ICrd<TModel, TId> storage, GetIdDelegate<TModel, TId> getIdDelegate,
            IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null,
            AutoCacheOptions options = null)
        :base(storage, getIdDelegate, cache, flushCacheDelegateAsync, options)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var createdItem = await _storage.CreateAndReturnAsync(item, token);
            await CacheSetAsync(createdItem, token);
            return createdItem;
        }

        /// <inheritdoc />
        public async Task<TId> CreateAsync(TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var id = await _storage.CreateAsync(item, token);
            await CacheMaybeSetAsync(id, token);
            return id;
        }

        /// <inheritdoc />
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var createdItem = await _storage.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
            await CacheSetByIdAsync(id, createdItem, token);
            return createdItem;
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            await _storage.CreateWithSpecifiedIdAsync(id, item, token);
            await CacheMaybeSetAsync(id, token);
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            var task1 = FlushAsync(token);
            var task2 = _storage.DeleteAllAsync(token);
            await Task.WhenAll(task1, task2);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var task1 = CacheRemoveByIdAsync(id, token);
            var task2 = _storage.DeleteAsync(id, token);
            await Task.WhenAll(task1, task2);
        }

        /// <summary>
        /// Read and cache the item, but only if we have an aggressive caching strategy.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        protected async Task CacheMaybeSetAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            async Task<bool> IsAlreadyCachedAndGetIsOkToUpdate()
            {
                return Options.DoGetToUpdate && await CacheItemExistsAsync(id, token);
            }

            var getAndSave = Options.SaveAll || await IsAlreadyCachedAndGetIsOkToUpdate();
            if (!getAndSave) return;
            var item = await _storage.ReadAsync(id, token);
            await CacheSetByIdAsync(id, item, token);
        }

        /// <summary>
        /// Check if an item exists in the cache.
        /// </summary>
        /// <param name="id">The id for the item</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        protected async Task<bool> CacheItemExistsAsync(TId id, CancellationToken token)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var key = GetCacheKeyFromId(id);
            var cachedItem = await Cache.GetAsync(key, token);
            return cachedItem != null;
        }
    }
}
