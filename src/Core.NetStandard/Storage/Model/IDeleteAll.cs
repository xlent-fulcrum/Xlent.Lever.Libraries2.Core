using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Delete items of type <see cref="IStorableItem{TId}"/>.
    /// </summary>
    public interface IDeleteAll
    {
        /// <summary>
        /// Delete all the items from storage.
        /// </summary>
        /// <remarks>
        /// The implementor of this method can decide that it is not a valid method to expose.
        /// In that case, the method should throw a <see cref="FulcrumNotImplementedException"/>.
        /// </remarks>
        Task DeleteAllAsync();
    }
}
