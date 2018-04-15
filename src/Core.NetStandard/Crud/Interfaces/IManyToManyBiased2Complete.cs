using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    public interface IManyToManyBiased2Complete<in TManyToManyModelCreate, TManyToManyModel, TManyModel, TId> : ICrud<TManyToManyModelCreate, TManyToManyModel, TId>, IManyToManyBiased2<TManyModel, TId>
    {
        /// <summary>
        /// Find all items with reference 2 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<PageEnvelope<TManyToManyModel>> ReadByReference2WithPagingAsync(TId id, int offset, int? limit = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Find all items reference 2 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the items for.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<IEnumerable<TManyToManyModel>> ReadByReference2Async(TId id, int limit = int.MaxValue, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Delete all items reference 2 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to delete the items for.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task DeleteByReference2Async(TId id, CancellationToken token = default(CancellationToken));
    }
}
