using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Read items of type <see cref="IStorableItem{TId}"/>.
    /// </summary>
    /// <typeparam name="TStorable">The type of objects to read from persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IStorableItem{TId}.Id"/> property.</typeparam>
    public interface IReadAll<TStorable, TId>
        where TStorable : IStorableItem<TId>
    {
        /// <summary>
        /// Reads all the items from storage.
        /// </summary>
        /// <returns>A list of the found objects. Can be empty, but never null.</returns>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <remarks>
        /// The implementor of this method can decide that it is not a valid method to expose.
        /// In that case, the method should throw a <see cref="FulcrumNotImplementedException"/>.
        /// </remarks>
        Task<IPageEnvelope<TStorable, TId>> ReadAllAsync(int offset = 0, int? limit = null);
    }
}
