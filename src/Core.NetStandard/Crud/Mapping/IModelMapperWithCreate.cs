using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Crud.Mapping
{
    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModelCreate">The client model to create an item.</typeparam>
    /// <typeparam name="TClientModel">The client model.</typeparam>
    /// <typeparam name="TServerLogic">Server logic to use if needed for the mapping.</typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    public interface IModelMapperWithCreate<in TClientModelCreate, TClientModel, in TServerLogic, TServerModel> : IModelMapper<TClientModel, TServerLogic, TServerModel>
    {
        /// <summary>
        /// Create a server model from a client model (<paramref name="source"/>).
        /// </summary>
        TServerModel MapToServer(TClientModelCreate source);
    }
}
