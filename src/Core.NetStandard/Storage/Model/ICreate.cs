using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Can create items of type <see cref="IIdentifiable{TId}"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of objects to create in persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IIdentifiable{TId}.Id"/> property.</typeparam>
    public interface ICreate<TItem, TId>
    {
        /// <summary>
        /// Creates a new item in storage and returns the new Id.
        /// </summary>
        /// <param name="item">The item to store.</param>
        /// <returns>The new id for the created object.</returns>
        Task<TId> CreateAsync(TItem item);

        /// <summary>
        /// Creates a new item in storage and returns the final result.
        /// </summary>
        /// <param name="item">The item to store.</param>
        /// <returns>The new item as it was saved, see remarks below.</returns>
        /// <remarks>
        /// If the returned type implements <see cref="IIdentifiable{TId}"/>, then the <see cref="IIdentifiable{TId}.Id"/> is updated with the new id. 
        /// If it implements <see cref="IOptimisticConcurrencyControlByETag"/>, then the <see cref="IOptimisticConcurrencyControlByETag.Etag"/> is updated..
        /// </remarks>
        /// <seealso cref="IOptimisticConcurrencyControlByETag"/>
        /// <seealso cref="IIdentifiable{TId}"/>
        Task<TItem> CreateAndReturnAsync(TItem item);
    }
}
