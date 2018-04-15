using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Can create items of type <see cref="IUniquelyIdentifiable{TId}"/>.
    /// </summary>
    /// <typeparam name="TModelCreate">The type for creating objects in persistant storage.</typeparam>
    /// <typeparam name="TModelReturned">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IUniquelyIdentifiable{TId}.Id"/> property.</typeparam>
    public interface ICreate<in TModelCreate, TModelReturned, TId>
    {
        /// <summary>
        /// Creates a new item in storage and returns the new Id.
        /// </summary>
        /// <param name="item">The item to store.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <returns>The new id for the created object.</returns>
        Task<TId> CreateAsync(TModelCreate item, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Creates a new item in storage and returns the final result.
        /// </summary>
        /// <param name="item">The item to store.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <returns>The new item as it was saved, see remarks below.</returns>
        /// <remarks>
        /// If the returned type implements <see cref="IUniquelyIdentifiable{TId}"/>, then the <see cref="IUniquelyIdentifiable{TId}.Id"/> is updated with the new id. 
        /// If it implements <see cref="IOptimisticConcurrencyControlByETag"/>, then the <see cref="IOptimisticConcurrencyControlByETag.Etag"/> is updated..
        /// </remarks>
        /// <seealso cref="IOptimisticConcurrencyControlByETag"/>
        /// <seealso cref="IUniquelyIdentifiable{TId}"/>
        Task<TModelReturned> CreateAndReturnAsync(TModelCreate item, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Same as <see cref="CreateAsync"/>, but you can specify the new id.
        /// </summary>
        /// <param name="id">The id to use for the new item.</param>
        /// <param name="item">The item to create in storage.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <returns>The newly created item.</returns>
        Task CreateWithSpecifiedIdAsync(TId id, TModelCreate item, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Same as <see cref="CreateAndReturnAsync"/>, but you can specify the new id.
        /// </summary>
        /// <param name="id">The id to use for the new item.</param>
        /// <param name="item">The item to store.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <returns>The new item as it was saved, see remarks below.</returns>
        /// <remarks>
        /// If the returned type implements <see cref="IUniquelyIdentifiable{TId}"/>, then the <see cref="IUniquelyIdentifiable{TId}.Id"/> is updated with the new id. 
        /// If it implements <see cref="IOptimisticConcurrencyControlByETag"/>, then the <see cref="IOptimisticConcurrencyControlByETag.Etag"/> is updated..
        /// </remarks>
        /// <seealso cref="IOptimisticConcurrencyControlByETag"/>
        /// <seealso cref="IUniquelyIdentifiable{TId}"/>
        Task<TModelReturned> CreateWithSpecifiedIdAndReturnAsync(TId id, TModelCreate item, CancellationToken token = default(CancellationToken));
    }
}
