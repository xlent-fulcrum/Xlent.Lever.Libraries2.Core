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
    public class ManyToOneAutoCache<TManyModel, TId> : AutoCacheBase<TManyModel, TId>, IManyToOne<TManyModel, TId>
    {
        private readonly IManyToOne<TManyModel, TId> _storage;
        /// <summary>
        /// Constructor for TOneModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public ManyToOneAutoCache(IManyToOne<TManyModel, TId> storage, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
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
        public ManyToOneAutoCache(IManyToOne<TManyModel, TId> storage, GetIdDelegate<TManyModel, TId> getIdDelegate, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
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
