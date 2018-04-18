using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModelCreate">The client model to create an item.</typeparam>
    /// <typeparam name="TClientId"></typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    public interface ICreateModelMapper<in TClientModelCreate, in TClientId, TServerModel>
    {
        /// <summary>
        /// Create a server model from a client model (<paramref name="source"/>).
        /// </summary>
        Task<TServerModel> CreateAndReturnAsync(TClientModelCreate source, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Create a server model from a client model (<paramref name="source"/>).
        /// </summary>
        Task<TServerModel> CreateWithSpecifiedIdAndReturnAsync(TClientId id, TClientModelCreate source, CancellationToken token = default(CancellationToken));
    }
}
