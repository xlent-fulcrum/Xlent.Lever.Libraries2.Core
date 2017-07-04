using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Delete an item of type <see cref="IStorableItem{TId}"/>.
    /// </summary>
    /// <typeparam name="TId">The type for the <see cref="IStorableItem{TId}.Id"/> property.</typeparam>
    public interface IDelete<in TId>
    {
        /// <summary>
        /// Deletes the item uniquely identified by <paramref name="id"/> from storage.
        /// </summary>
        Task DeleteAsync(TId id);
    }
}
