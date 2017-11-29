using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Read items"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects to read from persistant storage.</typeparam>
    public interface IReadAll<T>
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
        Task<PageEnvelope<T>> ReadAllAsync(int offset = 0, int? limit = null);
    }
}
