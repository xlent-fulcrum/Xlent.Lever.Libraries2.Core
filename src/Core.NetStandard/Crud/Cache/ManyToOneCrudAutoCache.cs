using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Cache
{
    /// <summary>
    /// Use this to put an "intelligent" cache between you and your ICrud storage.
    /// </summary>
    /// <typeparam name="TManyModel">The model for the children that each points out a parent.</typeparam>
    /// <typeparam name="TId">The type for the id field of the models.</typeparam>
    public class ManyToOneCrudAutoCache<TManyModel, TId> : ManyToOneCrudAutoCache<TManyModel, TManyModel, TId>,
        ICrud<TManyModel, TId>, IManyToOneCrud<TManyModel, TId>
    {
        /// <summary>
        /// Constructor for TOneModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public ManyToOneCrudAutoCache(IManyToOneCrud<TManyModel, TId> storage, IDistributedCache cache,
            FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
            : this(storage, item => ((IUniquelyIdentifiable<TId>)item).Id, cache, flushCacheDelegateAsync, options)
        {
        }


        /// <summary>
        /// Constructor for TOneModel that does not implement <see cref="IUniquelyIdentifiable{TId}"/>, or when you want to specify your own GetKey() method.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="getIdDelegate"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public ManyToOneCrudAutoCache(IManyToOneCrud<TManyModel, TId> storage,
            GetIdDelegate<TManyModel, TId> getIdDelegate, IDistributedCache cache,
            FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
            : base(storage, getIdDelegate, cache, flushCacheDelegateAsync, options)
        {
        }
    }

    /// <summary>
    /// Use this to put an "intelligent" cache between you and your ICrud storage.
    /// </summary>
    /// <typeparam name="TManyModelCreate"></typeparam>
    /// <typeparam name="TManyModel">The model for the children that each points out a parent.</typeparam>
    /// <typeparam name="TId">The type for the id field of the models.</typeparam>
    public class ManyToOneCrudAutoCache<TManyModelCreate, TManyModel, TId> :
        CrudAutoCache<TManyModelCreate, TManyModel, TId>,
        IManyToOneCrud<TManyModelCreate, TManyModel, TId> where TManyModel : TManyModelCreate
    {
        private readonly IManyToOneCrud<TManyModelCreate, TManyModel, TId> _storage;

        /// <summary>
        /// Constructor for TOneModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public ManyToOneCrudAutoCache(IManyToOneCrud<TManyModelCreate, TManyModel, TId> storage,
            IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null,
            AutoCacheOptions options = null)
            : this(storage, item => ((IUniquelyIdentifiable<TId>)item).Id, cache, flushCacheDelegateAsync, options)
        {
        }


        /// <summary>
        /// Constructor for TOneModel that does not implement <see cref="IUniquelyIdentifiable{TId}"/>, or when you want to specify your own GetKey() method.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="getIdDelegate"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public ManyToOneCrudAutoCache(IManyToOneCrud<TManyModelCreate, TManyModel, TId> storage,
            GetIdDelegate<TManyModel, TId> getIdDelegate, IDistributedCache cache,
            FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
            : base(storage, getIdDelegate, cache, flushCacheDelegateAsync, options)
        {
            _storage = storage;
        }

        /// <summary>
        /// True while a background thread is active saving results from a ReadAll() operation.
        /// </summary>
        protected bool IsCollectionOperationActive(TId parentId)
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            return IsCollectionOperationActive(CacheKeyForChildrenCollection(parentId));
        }

        /// <summary>
        /// Wait until any background thread is active saving results from a ReadAll() operation.
        /// </summary>
        public async Task DelayUntilNoOperationActiveAsync(TId parentId)
        {
            var count = 0;
            while (count++ < 5 && !IsCollectionOperationActive(parentId)) await Task.Delay(TimeSpan.FromMilliseconds(1));
            while (IsCollectionOperationActive(parentId)) await Task.Delay(TimeSpan.FromMilliseconds(10));
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(TId masterId, CancellationToken token = default(CancellationToken))
        {
            await _storage.DeleteChildrenAsync(masterId, token);
            await RemoveCachedChildrenInBackgroundAsync(masterId, token);
        }

        private async Task RemoveCachedChildrenInBackgroundAsync(TId parentId, CancellationToken token = default(CancellationToken))
        {
            var key = CacheKeyForChildrenCollection(parentId);
            await RemoveCacheItemsInBackgroundAsync(key, async () => await CacheGetAsync(int.MaxValue, key, token));
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TManyModel>> ReadChildrenWithPagingAsync(TId parentId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(limit));
            if (limit == null) limit = PageInfo.DefaultLimit;
            InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            var key = CacheKeyForChildrenCollection(parentId);
            var result = await CacheGetAsync(offset, limit.Value, key, token);
            if (result != null) return result;
            result = await _storage.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
            if (result?.Data == null) return null;
            CacheItemsInBackground(result, limit.Value, key);
            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TManyModel>> ReadChildrenAsync(TId parentId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            var key = CacheKeyForChildrenCollection(parentId);
            var itemsArray = await CacheGetAsync(limit, key, token);
            if (itemsArray != null) return itemsArray;
            var itemsCollection = await _storage.ReadChildrenAsync(parentId, limit, token);
            itemsArray = itemsCollection as TManyModel[] ?? itemsCollection.ToArray();
            CacheItemsInBackground(itemsArray, limit, key);
            return itemsArray;
        }

        private static string CacheKeyForChildrenCollection(TId parentId)
        {
            return $"childrenOf-{parentId}";
        }
    }
}
