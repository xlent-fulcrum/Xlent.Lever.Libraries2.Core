using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Update an item of type <see cref="IStorableItem{TId}"/>.
    /// </summary>
    /// <typeparam name="TStorable">The type of objects to update in persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IStorableItem{TId}.Id"/> property.</typeparam>
    public interface IUpdate<TStorable, TId>
        where TStorable : IStorableItem<TId>
    {
        /// <summary>
        /// Updates the item uniquely identified by <paramref name="item.Id"/> in storage.
        /// </summary>
        /// <param name="item">The new version of the item.</param>
        /// <remarks>
        /// The note about <see cref="IOptimisticConcurrencyControlByETag.ETag"/> are only valid if the <see cref="IStorableItem{TId}"/> type implements <seealso cref="IOptimisticConcurrencyControlByETag"/>.
        /// </remarks>
        /// <exception cref="FulcrumNotFoundException">Thrown if the <see cref="IStorableItem{TId}.Id"/> for <paramref name="item"/> could not be found.</exception>
        /// <exception cref="FulcrumConflictException">Thrown if the <see cref="IOptimisticConcurrencyControlByETag.ETag"/> for <paramref name="item"/> was outdated.</exception>
        Task UpdateAsync(TStorable item);

        /// <summary>
        /// Updates the item uniquely identified by <paramref name="item.Id"/> in storage.
        /// </summary>
        /// <param name="item">The new version of the item.</param>
        /// <returns>The updated item as it was saved, including an updated <see cref="IOptimisticConcurrencyControlByETag.ETag"/></returns>
        /// <remarks>
        /// The notes about <see cref="IOptimisticConcurrencyControlByETag.ETag"/> are only valid if the <see cref="IStorableItem{TId}"/> type implements <see cref="IOptimisticConcurrencyControlByETag"/>.
        /// </remarks>
        /// <exception cref="FulcrumNotFoundException">Thrown if the <see cref="IStorableItem{TId}.Id"/> for <paramref name="item"/> could not be found.</exception>
        /// <exception cref="FulcrumConflictException">Thrown if the <see cref="IOptimisticConcurrencyControlByETag.ETag"/> for <paramref name="item"/> was outdated.</exception>
        Task<TStorable> UpdateAndReturnAsync(TStorable item);
    }
}
