using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting groups of objects.
    /// </summary>
    public interface IManyToOneRelation<TManyModel, TOneModel, in TId>
    {
        /// <summary>
        /// Read all child items for a specific parent, <paramref name="parentId"/>.
        /// </summary>
        /// <param name="parentId">The specific parent to read the child items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<PageEnvelope<TManyModel>> ReadChildrenWithPagingAsync(TId parentId, int offset = 0, int? limit = null);

        /// <summary>
        /// Read all child items for a specific parent, <paramref name="parentId"/>.
        /// </summary>
        /// <param name="parentId">The specific parent to read the child items for.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<IEnumerable<TManyModel>> ReadChildrenAsync(TId parentId, int limit = int.MaxValue);

        /// <summary>
        /// Read the parent for the child <paramref name="childId"/>.
        /// </summary>
        /// <param name="childId">The specific child to read the parent item for.</param>
        Task<TOneModel> ReadParentAsync(TId childId);

        /// <summary>
        /// Delete all child items for a specific parent, <paramref name="parentId"/>.
        /// </summary>
        Task DeleteChildrenAsync(TId parentId);
    }
}
