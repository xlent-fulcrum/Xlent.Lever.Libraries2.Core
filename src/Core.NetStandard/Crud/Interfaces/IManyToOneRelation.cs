using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Functionality for persisting groups of objects.
    /// </summary>
    public interface IManyToOneRelation<TManyModel, in TId>
    {
        /// <summary>
        /// Read all child items for a specific parent, <paramref name="parentId"/>.
        /// </summary>
        /// <param name="parentId">The specific parent to read the child items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<PageEnvelope<TManyModel>> ReadChildrenWithPagingAsync(TId parentId, int offset, int? limit = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Read all child items for a specific parent, <paramref name="parentId"/>.
        /// </summary>
        /// <param name="parentId">The specific parent to read the child items for.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<IEnumerable<TManyModel>> ReadChildrenAsync(TId parentId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Delete all child items for a specific parent, <paramref name="masterId"/>.
        /// </summary>
        Task DeleteChildrenAsync(TId masterId, CancellationToken token = default(CancellationToken));
    }
}
