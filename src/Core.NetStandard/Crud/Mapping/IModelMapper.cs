using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Mapping
{
    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModel">The client model.</typeparam>
    /// <typeparam name="TServerLogic">Server logic to use if needed for the mapping.</typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    public interface IModelMapper<TClientModel, in TServerLogic, TServerModel>
    {
        /// <summary>
        /// Create a client model from a server model (<paramref name="source"/>). Use <paramref name="logic"/> if you need to access more server objects.
        /// </summary>
        Task<TClientModel> MapFromServerAsync(TServerModel source, TServerLogic logic, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Create a server model from a client model (<paramref name="source"/>). This is used when no side effects are allowed.
        /// </summary>
        TServerModel MapToServer(TClientModel source);

        /// <summary>
        /// Create a server model from a client model (<paramref name="source"/>). Use <paramref name="logic"/> if you need to access more server objects.
        /// </summary>
        Task<TServerModel> MapToServerAsync(TClientModel source, TServerLogic logic, CancellationToken token = default(CancellationToken));
    }
}
