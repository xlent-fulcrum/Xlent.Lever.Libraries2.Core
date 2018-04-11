using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Delete an item of type <see cref="IUniquelyIdentifiable{TId}"/>.
    /// </summary>
    /// <typeparam name="TId">The type for the <see cref="IUniquelyIdentifiable{TId}.Id"/> property.</typeparam>
    public interface IDelete<in TId>
    {
        /// <summary>
        /// Deletes the item uniquely identified by <paramref name="id"/> from storage.
        /// </summary>
        /// <param name="id">The id of the item that should be deleted.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task DeleteAsync(TId id, CancellationToken token = default(CancellationToken));
    }
}
