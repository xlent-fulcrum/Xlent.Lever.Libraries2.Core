using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModel">The client model.</typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    public interface IReadModelMapper<TClientModel, in TServerModel>
    {
        /// <summary>
        /// Create a client model from a server model (<paramref name="source"/>).
        /// </summary>
        Task<TClientModel> MapFromServerAsync(TServerModel source, CancellationToken token = default(CancellationToken));
    }
}
