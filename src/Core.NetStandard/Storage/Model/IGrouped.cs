using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting groups of objects.
    /// </summary>
    public interface IGrouped<TStorableItem, TId, in TGroup>
        where TStorableItem : IStorableItem<TId>
    {
        /// <summary>
        /// Create a new <paramref name="item"/> in the group <paramref name="groupId"/>.
        /// </summary>
        /// <returns></returns>
        Task<TStorableItem> CreateAsync(TGroup groupId, TStorableItem item);

        /// <summary>
        /// Read all items for a specific group, <paramref name="groupId"/>.
        /// </summary>
        /// <param name="groupId">The specific group to read the items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<IPageEnvelope<TStorableItem, TId>> ReadAllAsync(TGroup groupId, int offset = 0, int? limit = null);

        /// <summary>
        /// Delete all items for a specific group, <paramref name="groupId"/>.
        /// </summary>
        Task DeleteAllAsync(TGroup groupId);
    }
}
