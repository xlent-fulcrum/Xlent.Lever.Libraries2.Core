using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// Abstract base class that has a default implementation for the methods <see cref="CrdBase{TItem,TId}.CreateAndReturnAsync"/>,
    /// <see cref="CrdBase{TItem,TId}.DeleteAllAsync"/> and <see cref="UpdateAndReturnAsync"/>.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class CrudBase<TItem, TId> :  CrdBase<TItem, TId>, ICrud<TItem, TId>
    {
        /// <inheritdoc />
        public abstract Task UpdateAsync(TId id, TItem item);

        /// <inheritdoc />
        public virtual async Task<TItem> UpdateAndReturnAsync(TId id, TItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            await UpdateAsync(id, item);
            return await ReadAsync(id);
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IOptimisticConcurrencyControlByETag"/>
        /// then the old value is read using <see cref="CrdBase{TItem,TId}.ReadAsync"/> and the values are verified to be equal.
        /// The Etag of the item is then set to a new value.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual async Task MaybeVerifyEtagForUpdateAsync(TId id, TItem item)
        {
            if (item is IOptimisticConcurrencyControlByETag etaggable)
            {
                var oldItem = await ReadAsync(id);
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
