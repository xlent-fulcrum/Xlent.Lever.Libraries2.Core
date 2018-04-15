using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Helpers
{
    /// <summary>
    /// Abstract base class that has a default implementation for the methods <see cref="CrdBase{TModelCreate, TModel,TId}.CreateAndReturnAsync"/>,
    /// <see cref="CrdBase{TModelCreate, TModel,TId}.DeleteAllAsync"/> and <see cref="CrudBase{TModelCreate,TModel,TId}.UpdateAndReturnAsync"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class CrudBase<TModel, TId> : CrudBase<TModel, TModel, TId>,
        ICrud<TModel, TId>
    {
    }

    /// <summary>
    /// Abstract base class that has a default implementation for the methods <see cref="CrdBase{TModelCreate, TModel,TId}.CreateAndReturnAsync"/>,
    /// <see cref="CrdBase{TModelCreate, TModel,TId}.DeleteAllAsync"/> and <see cref="UpdateAndReturnAsync"/>.
    /// </summary>
    /// <typeparam name="TModelCreate">The type for creating objects in persistant storage.</typeparam>
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class CrudBase<TModelCreate, TModel, TId> : CrdBase<TModelCreate, TModel, TId>, ICrud<TModelCreate, TModel, TId> where TModel : TModelCreate
    {
        /// <inheritdoc />
        public abstract Task UpdateAsync(TId id, TModel item, CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task<TModel> UpdateAndReturnAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            await UpdateAsync(id, item, token);
            return await ReadAsync(id, token);
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IOptimisticConcurrencyControlByETag"/>
        /// then the old value is read using <see cref="ReadBase{TModel,TId}.ReadAsync"/> and the values are verified to be equal.
        /// The Etag of the item is then set to a new value.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <returns></returns>
        protected virtual async Task MaybeVerifyEtagForUpdateAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            if (item is IOptimisticConcurrencyControlByETag etaggable)
            {
                var oldItem = await ReadAsync(id, token);
                if (oldItem != null)
                {
                    var oldEtag = (oldItem as IOptimisticConcurrencyControlByETag)?.Etag;
                    if (oldEtag?.ToLowerInvariant() != etaggable.Etag?.ToLowerInvariant())
                        throw new FulcrumConflictException($"The updated item ({item}) had an old ETag value.");
                }
            }
        }
    }
}
