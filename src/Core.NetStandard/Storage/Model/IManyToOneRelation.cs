using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting groups of objects.
    /// </summary>
    public interface IManyToOneRelation<TManyModel, in TId>
    {
        /// <summary>
        /// Read all child items for a specific parent, <paramref name="reference1Id"/>.
        /// </summary>
        /// <param name="reference1Id">The specific parent to read the child items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<PageEnvelope<TManyModel>> ReadChildrenWithPagingAsync(TId reference1Id, int offset, int? limit = null);

        /// <summary>
        /// Read all child items for a specific parent, <paramref name="parentId"/>.
        /// </summary>
        /// <param name="parentId">The specific parent to read the child items for.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<IEnumerable<TManyModel>> ReadChildrenAsync(TId parentId, int limit = int.MaxValue);

        /// <summary>
        /// Delete all child items for a specific parent, <paramref name="parentId"/>.
        /// </summary>
        Task DeleteChildrenAsync(TId parentId);
    }
}
