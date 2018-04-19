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
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public class CrudAutoCache<TModel, TId> : CrudAutoCache<TModel, TModel, TId>,
        ICrud<TModel, TId>
    {
        /// <summary>
        /// Constructor for TModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public CrudAutoCache(ICrud<TModel, TId> storage, IDistributedCache cache,
            FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
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
        public CrudAutoCache(ICrud<TModel, TId> storage, GetIdDelegate<TModel, TId> getIdDelegate,
            IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null,
            AutoCacheOptions options = null)
            : base(storage, getIdDelegate, cache, flushCacheDelegateAsync, options)
        {
        }
    }

    /// <summary>
    /// Use this to put an "intelligent" cache between you and your ICrud storage.
    /// </summary>
    /// <typeparam name="TModelCreate">The type for creating objects in persistant storage.</typeparam>
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public class CrudAutoCache<TModelCreate, TModel, TId> : CrdAutoCache<TModelCreate, TModel, TId>, ICrud<TModelCreate, TModel, TId> where TModel : TModelCreate
    {
        private readonly ICrud<TModelCreate, TModel, TId> _storage;

        /// <summary>
        /// Constructor for TModel that implements <see cref="IUniquelyIdentifiable{TId}"/>.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="cache"></param>
        /// <param name="flushCacheDelegateAsync"></param>
        /// <param name="options"></param>
        public CrudAutoCache(ICrud<TModelCreate, TModel, TId> storage, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
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
        public CrudAutoCache(ICrud<TModelCreate, TModel, TId> storage, GetIdDelegate<TModel, TId> getIdDelegate, IDistributedCache cache, FlushCacheDelegateAsync flushCacheDelegateAsync = null, AutoCacheOptions options = null)
            : base(storage, getIdDelegate, cache, flushCacheDelegateAsync, options)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<TModel> UpdateAndReturnAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var updatedItem = await _storage.UpdateAndReturnAsync(id, item, token);
            await CacheSetByIdAsync(id, updatedItem, token);
            return updatedItem;

        }

        /// <inheritdoc />
        public async Task UpdateAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            await _storage.UpdateAsync(id, item, token);
            await CacheMaybeSetAsync(id, token);
        }
    }
}
