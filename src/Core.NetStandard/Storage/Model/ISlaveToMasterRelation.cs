using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting objects that has no life of their own, but are only relevant with their master.
    /// Examples: A list of rows on an invoice, a list of attributes of an object.
    /// </summary>
    public interface ISlaveToMasterRelation<TModel, TSlaveId, in TMasterId> : IManyToOneRelation<TModel, TMasterId>
    {
        /// <summary>
        /// 
        /// Create a new slave <paramref name="item"/> for the master <paramref name="masterId"/>.
        /// </summary>
        /// <returns>The new id for the created object.</returns>
        Task<TSlaveId> CreateAsync(TMasterId masterId, TModel item, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// 
        /// Create a new slave <paramref name="item"/> with the speficied <paramref name="id"/> for the master <paramref name="masterId"/>.
        /// </summary>
        Task CreateWithSpecifiedIdAsync(TMasterId masterId, TSlaveId id, TModel item, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Create a new slave <paramref name="item"/> for the master <paramref name="masterId"/>.
        /// </summary>
        /// <returns>The new item as it was saved, including an updated <see cref="IUniquelyIdentifiable{TId}.Id"/> and (if it exists) an updated <see cref="IOptimisticConcurrencyControlByETag.Etag"/>.</returns>
        Task<TModel> CreateAndReturnAsync(TMasterId masterId, TModel item, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Create a new slave <paramref name="item"/> with the speficied <paramref name="id"/> for the master <paramref name="masterId"/>.
        /// </summary>
        /// <returns>The new item as it was saved, including an updated <see cref="IUniquelyIdentifiable{TId}.Id"/> and (if it exists) an updated <see cref="IOptimisticConcurrencyControlByETag.Etag"/>.</returns>
        Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TMasterId masterId, TSlaveId id, TModel item, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns the slave item uniquely identified by <paramref name="id"/> of the master <paramref name="masterId"/>.
        /// </summary>
        /// <param name="masterId">The specific group to read the items for.</param>
        /// <param name="id">The </param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<TModel> ReadAsync(TMasterId masterId, TSlaveId id, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Delete the slave item uniquely identified by <paramref name="id"/> of the master <paramref name="masterId"/>.
        /// </summary>
        /// <param name="masterId">The specific group to read the items for.</param>
        /// <param name="id">The </param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task DeleteAsync(TMasterId masterId, TSlaveId id, CancellationToken token = default(CancellationToken));
    }
}
