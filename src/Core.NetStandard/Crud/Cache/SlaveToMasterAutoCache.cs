using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Cache
{
    /// <inheritdoc cref="SlaveToMasterAutoCache{TManyModelCreate,TManyModel,TId}" />
    public class SlaveToMasterAutoCache<TManyModel, TId> :
        SlaveToMasterAutoCache<TManyModel, TManyModel, TId>, ISlaveToMaster<TManyModel, TId>
    {
        /// <summary>
        /// Constructor for TOneModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public SlaveToMasterAutoCache(ISlaveToMaster<TManyModel, TId> storage,
            IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null,
            AutoCacheOptions options = null)
            : this(storage, item => ((IUniquelyIdentifiable<SlaveToMasterId<TId>>) item).Id, cache,
                flushCacheDelegateAsync, options)
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
        public SlaveToMasterAutoCache(ISlaveToMaster<TManyModel, TId> storage,
            GetIdDelegate<TManyModel, SlaveToMasterId<TId>> getIdDelegate, IDistributedCache cache,
            FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
            : base(storage, getIdDelegate, cache, flushCacheDelegateAsync, options)
        {
        }
    }

    /// <summary>
    /// Use this to put an "intelligent" cache between you and your ICrud storage.
    /// </summary>
    /// <typeparam name="TManyModelCreate">The model to use when creating objects.</typeparam>
    /// <typeparam name="TManyModel">The model for the children that each points out a parent.</typeparam>
    /// <typeparam name="TId">The type for the id field of the models.</typeparam>
    public class SlaveToMasterAutoCache<TManyModelCreate, TManyModel, TId> : AutoCacheBase<TManyModel, SlaveToMasterId<TId>>, ISlaveToMaster<TManyModelCreate, TManyModel, TId>
            where TManyModel : TManyModelCreate
        {
            private readonly ISlaveToMaster<TManyModelCreate, TManyModel, TId> _storage;
            /// <summary>
            /// Constructor for TOneModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
            /// </summary>
            /// <param name="storage"></param>
            /// <param name="cache"></param>
            /// <param name="flushCacheDelegateAsync"></param>
            /// <param name="options"></param>
            public SlaveToMasterAutoCache(ISlaveToMaster<TManyModelCreate, TManyModel, TId> storage, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
            : this(storage, item => ((IUniquelyIdentifiable<SlaveToMasterId<TId>>)item).Id, cache, flushCacheDelegateAsync, options)
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
            public SlaveToMasterAutoCache(ISlaveToMaster<TManyModelCreate, TManyModel, TId> storage, GetIdDelegate<TManyModel, SlaveToMasterId<TId>> getIdDelegate, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
                : base(getIdDelegate, cache, flushCacheDelegateAsync, options)
            {
                _storage = storage;
            }

            /// <summary>
            /// True while a background thread is active saving results from a ReadAll() operation.
            /// </summary>
            public bool IsCollectionOperationActive(TId parentId)
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            return IsCollectionOperationActive(CacheKeyForChildrenCollection(parentId));
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
        public async Task<IEnumerable<TManyModel>> ReadChildrenAsync(TId masterId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            var key = CacheKeyForChildrenCollection(masterId);
            var itemsArray = await CacheGetAsync(limit, key, token);
            if (itemsArray != null) return itemsArray;
            var itemsCollection = await _storage.ReadChildrenAsync(masterId, limit, token);
            itemsArray = itemsCollection as TManyModel[] ?? itemsCollection.ToArray();
            CacheItemsInBackground(itemsArray, limit, key);
            return itemsArray;
        }

        private static string CacheKeyForChildrenCollection(TId parentId)
        {
            return $"childrenOf-{parentId}";
        }

        /// <inheritdoc />
        public Task<SlaveToMasterId<TId>> CreateAsync(TId masterId, TManyModelCreate item, CancellationToken token = default(CancellationToken))
        {
            return _storage.CreateAsync(masterId, item, token);
        }

        /// <inheritdoc />
        public Task<TManyModel> CreateAndReturnAsync(TId masterId, TManyModelCreate item, CancellationToken token = default(CancellationToken))
        {
            return _storage.CreateAndReturnAsync(masterId, item, token);
        }

        /// <inheritdoc />
        public Task CreateWithSpecifiedIdAsync(SlaveToMasterId<TId> id, TManyModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            return _storage.CreateWithSpecifiedIdAsync(id, item, token);
        }
        
        /// <inheritdoc />
        public Task<TManyModel> CreateWithSpecifiedIdAndReturnAsync(SlaveToMasterId<TId> id, TManyModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            return _storage.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
        }

        /// <inheritdoc />
        public Task DeleteChildrenAsync(TId masterId, CancellationToken token = default(CancellationToken))
        {
            var task1 =  _storage.DeleteChildrenAsync(masterId, token);
            var task2 = RemoveCachedChildrenInBackgroundAsync(masterId, token);
            return Task.WhenAll(task1, task2);
        }
    }
}
