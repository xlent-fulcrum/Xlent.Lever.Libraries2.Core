using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Read an item of type <see cref="IStorableItem{TId}"/>.
    /// </summary>
    /// <typeparam name="TStorable">The type of objects to read from persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IStorableItem{TId}.Id"/> property.</typeparam>
    public interface IRead<TStorable, in TId>
        where TStorable : IStorableItem<TId>
    {
        /// <summary>
        /// Returns the item uniquely identified by <paramref name="id"/> from storage.
        /// </summary>
        /// <returns>The found item.</returns>
        /// <exception cref="FulcrumNotFoundException">Thrown if the <paramref name="id"/> could not be found.</exception>
        Task<TStorable> ReadAsync(TId id);
    }
}
