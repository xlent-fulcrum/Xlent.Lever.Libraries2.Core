using System.Collections.Generic;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Read items"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of objects to read from persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the id of the object.</typeparam>
    public interface IReadAll<TModel, in TId> : IRead<TModel, TId>
    {
        /// <summary>
        /// Reads all the items from storage and return them as pages.
        /// </summary>
        /// <returns>A page of the found items.</returns>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <remarks>
        /// The implementor of this method can decide that it is not a valid method to expose.
        /// In that case, the method should throw a <see cref="FulcrumNotImplementedException"/>.
        /// </remarks>
        Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset = 0, int? limit = null);

        /// <summary>
        /// Reads all the items from storage and return them as a collection of items.
        /// </summary>
        /// <returns>A list of the found objects. Can be empty, but never null.</returns>
        /// <param name="limit">The maximum number of items to return. 0 means no limit.</param>
        /// <remarks>
        /// The implementor of this method can decide that it is not a valid method to expose.
        /// In that case, the method should throw a <see cref="FulcrumNotImplementedException"/>.
        /// </remarks>
        Task<IEnumerable<TModel>> ReadAllAsync(int limit = 0);
    }
}
