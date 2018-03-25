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

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(TId parentId)
        {
            await _storage.DeleteChildrenAsync(parentId);
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(TId parentId, int offset = 0, int? limit = null)
        {
            return await _storage.ReadChildrenWithPagingAsync(parentId, offset, limit);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> ReadChildrenAsync(TId parentId, int limit = 0)
        {
            return await _storage.ReadChildrenAsync(parentId, limit);
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
    }
}
