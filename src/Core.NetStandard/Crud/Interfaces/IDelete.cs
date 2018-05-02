using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Delete items./>.
    /// </summary>
    public interface IDelete<in TId>
    {
        /// <summary>
        /// Deletes the item uniquely identified by <paramref name="id"/> from storage.
        /// </summary>
        /// <param name="id">The id of the item that should be deleted.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task DeleteAsync(TId id, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Delete all the items from storage.
        /// </summary>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <remarks>
        /// The implementor of this method can decide that it is not a valid method to expose.
        /// In that case, the method should throw a <see cref="FulcrumNotImplementedException"/>.
        /// </remarks>
        Task DeleteAllAsync(CancellationToken token = default(CancellationToken));
    }
}
