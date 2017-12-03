using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Read an item of type <see cref="IIdentifiable{TId}"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of objects to read from persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the id of the object.</typeparam>
    public interface IRead<TItem, in TId>
    {
        /// <summary>
        /// Returns the item uniquely identified by <paramref name="id"/> from storage.
        /// </summary>
        /// <returns>The found item.</returns>
        /// <exception cref="FulcrumNotFoundException">Thrown if the <paramref name="id"/> could not be found.</exception>
        Task<TItem> ReadAsync(TId id);
    }
}
