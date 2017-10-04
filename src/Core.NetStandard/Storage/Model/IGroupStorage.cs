using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting groups of objects.
    /// </summary>
    public interface IGroupStorage<TStorableItem, in TId>
        where TStorableItem : IStorableItem<TId>
    {
        /// <summary>
        /// Create a new <paramref name="item"/> in the group <paramref name="groupId"/>.
        /// </summary>
        /// <returns></returns>
        Task<TStorableItem> CreateAsync(TId groupId, TStorableItem item);

        /// <summary>
        /// Read all items for a specific group, <paramref name="groupId"/>.
        /// </summary>
        Task<IEnumerable<TStorableItem>> ReadAllAsync(TId groupId);

        /// <summary>
        /// Delete all items for a specific group, <paramref name="groupId"/>.
        /// </summary>
        Task DeleteAllAsync(TId groupId);
    }
}
