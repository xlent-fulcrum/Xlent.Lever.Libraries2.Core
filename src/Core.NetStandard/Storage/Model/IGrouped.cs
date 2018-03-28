using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting groups of objects.
    /// </summary>
    public interface IGrouped<TModel, TId, in TGroup>
    {
        /// <summary>
        /// 
        /// Create a new <paramref name="item"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        /// <returns>The new id for the created object.</returns>
        Task<TId> CreateAsync(TGroup groupValue, TModel item);

        /// <summary>
        /// 
        /// Create a new <paramref name="item"/> with the speficied <paramref name="id"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        Task CreateWithSpecifiedIdAsync(TGroup groupValue, TId id, TModel item);

        /// <summary>
        /// Create a new <paramref name="item"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        /// <returns>The new item as it was saved, including an updated <see cref="IUniquelyIdentifiable{TId}.Id"/> and (if it exists) an updated <see cref="IOptimisticConcurrencyControlByETag.Etag"/>.</returns>
        Task<TModel> CreateAndReturnAsync(TGroup groupValue, TModel item);

        /// <summary>
        /// Create a new <paramref name="item"/> with the speficied <paramref name="id"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        /// <returns>The new item as it was saved, including an updated <see cref="IUniquelyIdentifiable{TId}.Id"/> and (if it exists) an updated <see cref="IOptimisticConcurrencyControlByETag.Etag"/>.</returns>
        Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TGroup groupValue, TId id, TModel item);

        /// <summary>
        /// Read all items for a specific group, <paramref name="groupValue"/>.
        /// </summary>
        /// <param name="groupValue">The specific group to read the items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(TGroup groupValue, int offset = 0, int? limit = null);

        /// <summary>
        /// Read all items for a specific group, <paramref name="groupValue"/>.
        /// </summary>
        /// <param name="groupValue">The specific group to read the items for.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<IEnumerable<TModel>> ReadAllAsync(TGroup groupValue, int limit = int.MaxValue);

        /// <summary>
        /// Returns the item uniquely identified by <paramref name="id"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        /// <param name="groupValue">The specific group to read the items for.</param>
        /// <param name="id">The </param>
        Task<TModel> ReadAsync(TGroup groupValue, TId id);

        /// <summary>
        /// Delete the item uniquely identified by <paramref name="id"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        /// <param name="groupValue">The specific group to read the items for.</param>
        /// <param name="id">The </param>
        Task DeleteAsync(TGroup groupValue, TId id);

        /// <summary>
        /// Delete all items for a specific group, <paramref name="groupValue"/>.
        /// </summary>
        Task DeleteAllAsync(TGroup groupValue);
    }
}
