﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    /// <summary>
    /// Use this to put an "intelligent" cache between you and your ICrud storage.
    /// </summary>
    /// <typeparam name="TManyModel">The model for the children that each points out a parent.</typeparam>
    /// <typeparam name="TOneModel">The model for the parent.</typeparam>
    /// <typeparam name="TId">The type for the id field of the models.</typeparam>
    public class AutoCacheManyToOne<TManyModel, TOneModel, TId> : AutoCacheCrud<TManyModel, TId>, IManyToOneRelationComplete<TManyModel, TOneModel, TId>
    {
        private readonly IManyToOneRelationComplete<TManyModel, TOneModel, TId> _storage;
        /// <summary>
        /// Constructor for TOneModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public AutoCacheManyToOne(IManyToOneRelationComplete<TManyModel, TOneModel, TId> storage, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
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
        public AutoCacheManyToOne(IManyToOneRelationComplete<TManyModel, TOneModel, TId> storage, GetIdDelegate<TManyModel, TId> getIdDelegate, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
            : base(storage, getIdDelegate, cache, flushCacheDelegateAsync, options)
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

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(TId parentId)
        {
            await _storage.DeleteChildrenAsync(parentId);
            await RemoveCachedChildrenInBackgroundAsync(parentId);
        }

        private async Task RemoveCachedChildrenInBackgroundAsync(TId parentId)
        {
            var key = CacheKeyForChildrenCollection(parentId);
            await RemoveCacheItemsInBackgroundAsync(key, async () => await CacheGetAsync(int.MaxValue, key));
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TManyModel>> ReadChildrenWithPagingAsync(TId parentId, int offset = 0, int? limit = null)
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
        public async Task<IEnumerable<TManyModel>> ReadChildrenAsync(TId parentId, int limit = int.MaxValue)
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            var key = CacheKeyForChildrenCollection(parentId);
            var itemsArray = await CacheGetAsync(limit, key);
            if (itemsArray != null) return itemsArray;
            var itemsCollection = await _storage.ReadChildrenAsync(parentId, limit);
            itemsArray = itemsCollection as TManyModel[] ?? itemsCollection.ToArray();
            CacheItemsInBackground(itemsArray, limit, key);
            return itemsArray;
        }

        /// <inheritdoc />
        public async Task<TOneModel> ReadParentAsync(TId childId)
        {
            InternalContract.RequireNotDefaultValue(childId, nameof(childId));
            if (typeof(TManyModel) != typeof(TOneModel))
            {
                return await _storage.ReadParentAsync(childId);
            }

            var child = await ReadAsync(childId);
            InternalContract.RequireNotDefaultValue(childId, nameof(childId));
            var key = $"parentTo-{GetCacheKeyFromId(childId)}";
            var parent = ConvertSameType<TOneModel,TManyModel>(await CacheGetAsync(childId, key));
            if (parent != null) return parent;

            parent = await _storage.ReadParentAsync(childId);
            var parentAsManyModel = ConvertSameType<TManyModel,TOneModel>(parent);
            var task1 = CacheSetAsync(GetIdDelegate(parentAsManyModel), parentAsManyModel);
            var task2 = CacheSetAsync(childId, parentAsManyModel, key);
            await Task.WhenAll(task1, task2);
            return parent;
        }

        private TTarget ConvertSameType<TTarget, TSource>(TSource sourceType)
        {
            InternalContract.RequireNotNull(sourceType, nameof(sourceType));
            InternalContract.Require(sourceType is TTarget, $"Expected parameter {nameof(sourceType)} to be an instance of {typeof(TTarget).FullName}");
            var o = (object)sourceType;
            var targetType = (TTarget) o;
            FulcrumAssert.IsNotNull(targetType);
            return targetType;
        }

        private static string CacheKeyForChildrenCollection(TId parentId)
        {
            return $"childrenOf-{parentId}";
        }
    }
}