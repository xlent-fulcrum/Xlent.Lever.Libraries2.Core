using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModel">The client model.</typeparam>
    /// <typeparam name="TClientId"></typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    public interface ISlaveToMasterModelMapper<TClientModel, TClientId, TServerModel> :
        ISlaveToMasterModelMapper<TClientModel, TClientModel, TClientId, TServerModel>
    {
    }

    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModelCreate">The client model to create an item.</typeparam>
    /// <typeparam name="TClientModel">The client model.</typeparam>
    /// <typeparam name="TClientId"></typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    public interface ISlaveToMasterModelMapper<in TClientModelCreate, TClientModel, TClientId, TServerModel> :
        IRudModelMapper<TClientModel, TServerModel> where TClientModel : TClientModelCreate
    {
        /// <summary>
        /// Create a server model from a client model (<paramref name="source"/>).
        /// </summary>
        Task<TServerModel> CreateAndReturnAsync(TClientId masterId, TClientModelCreate source, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Create a server model from a client model (<paramref name="source"/>).
        /// </summary>
        Task<TServerModel> CreateWithSpecifiedIdAndReturnAsync(SlaveToMasterId<TClientId> id, TClientModelCreate source, CancellationToken token = default(CancellationToken));
    }
}
