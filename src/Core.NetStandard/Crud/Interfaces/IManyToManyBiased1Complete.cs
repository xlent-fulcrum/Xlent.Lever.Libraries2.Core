using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    public interface IManyToManyBiased1Complete<TManyToManyModel, TManyModel, TId> : ICrud<TManyToManyModel, TId>, IManyToManyBiased1<TManyModel, TId>
    {
        /// <summary>
        /// Find all items with reference 1 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<PageEnvelope<TManyToManyModel>> ReadByReference1WithPagingAsync(TId id, int offset, int? limit = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Find all items reference 1 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the items for.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<IEnumerable<TManyToManyModel>> ReadByReference1Async(TId id, int limit = int.MaxValue, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Delete all items reference 1 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to delete the items for.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task DeleteByReference1Async(TId id, CancellationToken token = default(CancellationToken));
    }
}
