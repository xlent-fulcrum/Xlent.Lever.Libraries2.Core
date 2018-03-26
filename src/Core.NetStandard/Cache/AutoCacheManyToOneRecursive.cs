using System.Collections.Generic;
using System.Linq;
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
    /// <typeparam name="TModel">The model for the parent.</typeparam>
    /// <typeparam name="TId">The type for the id field of the models.</typeparam>
    public class AutoCacheManyToOneRecursive<TModel, TId> : AutoCacheCrud<TModel, TId>, IManyToOneRecursiveRelationComplete<TModel, TId> where TModel : class
    {
        private readonly IManyToOneRecursiveRelationComplete<TModel, TId> _storage;

        /// <summary>
        /// Constructor for TModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public AutoCacheManyToOneRecursive(IManyToOneRecursiveRelationComplete<TModel, TId> storage, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
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
        public AutoCacheManyToOneRecursive(IManyToOneRecursiveRelationComplete<TModel, TId> storage, GetIdDelegate<TModel, TId> getIdDelegate, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
            : base(storage, getIdDelegate, cache, flushCacheDelegateAsync, options)
        {
            _storage = storage;
        }

        /// <summary>
        /// True while a background thread is active saving results from a ReadAll() operation.
        /// </summary>
        public bool GetSaveReadAllToCacheThreadIsActive(TId parentId)
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            return GetSaveReadAllToCacheThreadIsActive(CacheKeyForChildrenCollection(parentId));
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(TId parentId)
        {
            await _storage.DeleteChildrenAsync(parentId);
            // TODO: Do this work in the background
            var key = CacheKeyForChildrenCollection(parentId);
            var children = await CacheGetAsync(int.MaxValue, key);
            if (children == null) return;
            await Cache.RemoveAsync(key);
            foreach (var child in children)
            {
                var childKey = GetCacheKeyFromId(GetIdDelegate(child));
                await Cache.RemoveAsync(childKey);
            }
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(TId parentId, int offset = 0, int? limit = null)
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(limit));
            if (limit == null) limit = PageInfo.DefaultLimit;
            InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            var key = CacheKeyForChildrenCollection(parentId);
            var result = await CacheGetAsync(offset, limit.Value, key);
            if (result != null) return result;
            result = await _storage.ReadChildrenWithPagingAsync(parentId, offset, limit);
            if (result?.Data == null) return null;
            CacheItemsInBackground(result, limit.Value, key);
            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> ReadChildrenAsync(TId parentId, int limit = int.MaxValue)
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            var key = CacheKeyForChildrenCollection(parentId);
            var itemsArray = await CacheGetAsync(limit, key);
            if (itemsArray != null) return itemsArray;
            var itemsCollection = await _storage.ReadChildrenAsync(parentId, limit);
            itemsArray = itemsCollection as TModel[] ?? itemsCollection.ToArray();
            CacheItemsInBackground(itemsArray, limit, key);
            return itemsArray;
        }

        /// <inheritdoc />
        public async Task<TModel> ReadParentAsync(TId childId)
        {
            InternalContract.RequireNotDefaultValue(childId, nameof(childId));
            var key = $"parentTo-{GetCacheKeyFromId(childId)}";
            var item = await CacheGetAsync(childId, key);
            if (item != null) return item;
            item = await _storage.ReadParentAsync(childId);
            var task1 = CacheSetAsync(GetIdDelegate(item), item);
            var task2 = CacheSetAsync(childId, item, key);
            await Task.WhenAll(task1, task2);
            return item;
        }

        private static string CacheKeyForChildrenCollection(TId parentId)
        {
            return $"childrenOf-{parentId}";
        }
    }
}
