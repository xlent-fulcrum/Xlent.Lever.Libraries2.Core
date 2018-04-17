using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Crud.Mapping
{
    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModel">The client model.</typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    /// <typeparam name="TServerId"></typeparam>
    public interface IModelMapperWithCreate<TClientModel, TServerModel,
        in TServerId> : IModelMapperWithCreate<TClientModel, TClientModel, TServerModel, TServerId>
    {
    }

    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModelCreate">The client model to create an item.</typeparam>
    /// <typeparam name="TClientModel">The client model.</typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    /// <typeparam name="TServerId"></typeparam>
    public interface IModelMapperWithCreate<in TClientModelCreate, TClientModel, TServerModel, in TServerId> : IModelMapper<TClientModel, TServerModel>
    {
        /// <summary>
        /// Create a server model from a client model (<paramref name="source"/>).
        /// </summary>
        Task<TServerModel> CreateInServerAndReturnAsync(TClientModelCreate source, TServerId serverId = default(TServerId), CancellationToken token = default(CancellationToken));
    }
}
