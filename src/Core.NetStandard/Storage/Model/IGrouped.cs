using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting groups of objects.
    /// </summary>
    public interface IGrouped<TModel, TId, in TGroupId> : IManyToOneRelation<TModel, TGroupId>
    {
        /// <summary>
        /// 
        /// Create a new <paramref name="item"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        /// <returns>The new id for the created object.</returns>
        Task<TId> CreateAsync(TGroupId groupValue, TModel item);

        /// <summary>
        /// 
        /// Create a new <paramref name="item"/> with the speficied <paramref name="id"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        Task CreateWithSpecifiedIdAsync(TGroupId groupValue, TId id, TModel item);

        /// <summary>
        /// Create a new <paramref name="item"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        /// <returns>The new item as it was saved, including an updated <see cref="IUniquelyIdentifiable{TId}.Id"/> and (if it exists) an updated <see cref="IOptimisticConcurrencyControlByETag.Etag"/>.</returns>
        Task<TModel> CreateAndReturnAsync(TGroupId groupValue, TModel item);

        /// <summary>
        /// Create a new <paramref name="item"/> with the speficied <paramref name="id"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        /// <returns>The new item as it was saved, including an updated <see cref="IUniquelyIdentifiable{TId}.Id"/> and (if it exists) an updated <see cref="IOptimisticConcurrencyControlByETag.Etag"/>.</returns>
        Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TGroupId groupValue, TId id, TModel item);

        /// <summary>
        /// Returns the item uniquely identified by <paramref name="id"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        /// <param name="groupValue">The specific group to read the items for.</param>
        /// <param name="id">The </param>
        Task<TModel> ReadAsync(TGroupId groupValue, TId id);

        /// <summary>
        /// Delete the item uniquely identified by <paramref name="id"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        /// <param name="groupValue">The specific group to read the items for.</param>
        /// <param name="id">The </param>
        Task DeleteAsync(TGroupId groupValue, TId id);
    }
}
