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
        Task DeleteAsync(TId id);
    }
}
