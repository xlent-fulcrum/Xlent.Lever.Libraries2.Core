using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Functionality for persisting objects that has no life of their own, but are only relevant with their master.
    /// Examples: A list of rows on an invoice, a list of attributes of an object, the contact details of a person.
    /// </summary>
    public interface ISlaveToMasterRelation<TModel, TId> : IManyToOneRelation<TModel, TId>
    {
        /// <summary>
        /// 
        /// Create a new slave <paramref name="item"/> for the master <paramref name="masterId"/>.
        /// </summary>
        /// <returns>The new id for the created object.</returns>
        Task<TId> CreateAsync(TId masterId, TModel item, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// 
        /// Create a new slave <paramref name="item"/> with the speficied <paramref name="slaveId"/> for the master <paramref name="masterId"/>.
        /// </summary>
        Task CreateWithSpecifiedIdAsync(TId masterId, TId slaveId, TModel item, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Create a new slave <paramref name="item"/> for the master <paramref name="masterId"/>.
        /// </summary>
        /// <returns>The new item as it was saved, including an updated <see cref="IUniquelyIdentifiable{TId}.Id"/> and (if it exists) an updated <see cref="IOptimisticConcurrencyControlByETag.Etag"/>.</returns>
        Task<TModel> CreateAndReturnAsync(TId masterId, TModel item, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Create a new slave <paramref name="item"/> with the speficied <paramref name="slaveId"/> for the master <paramref name="masterId"/>.
        /// </summary>
        /// <returns>The new item as it was saved, including an updated <see cref="IUniquelyIdentifiable{TId}.Id"/> and (if it exists) an updated <see cref="IOptimisticConcurrencyControlByETag.Etag"/>.</returns>
        Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId masterId, TId slaveId, TModel item, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns the slave item uniquely identified by <paramref name="id"/> of the master <paramref name="masterId"/>.
        /// </summary>
        /// <param name="masterId">The specific group to read the items for.</param>
        /// <param name="slaveId">The </param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<TModel> ReadAsync(TId masterId, TId slaveId, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Delete the slave item uniquely identified by <paramref name="id"/> of the master <paramref name="masterId"/>.
        /// </summary>
        /// <param name="masterId">The specific group to read the items for.</param>
        /// <param name="slaveId">The </param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task DeleteAsync(TId masterId, TId slaveId, CancellationToken token = default(CancellationToken));
    }
}
