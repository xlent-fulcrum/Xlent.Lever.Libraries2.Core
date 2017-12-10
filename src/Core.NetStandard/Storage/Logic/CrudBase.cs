using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
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
    }
}
