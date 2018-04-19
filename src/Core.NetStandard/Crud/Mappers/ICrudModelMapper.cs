using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModel">The client model.</typeparam>
    /// <typeparam name="TClientId"></typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    public interface ICrudModelMapper<TClientModel, in TClientId, TServerModel> : 
        ICrudModelMapper<TClientModel, TClientModel, TClientId, TServerModel>, ICrdModelMapper<TClientModel, TClientId, TServerModel>
    {
    }

    /// <summary>
    /// Interface for mapping between client and server models.
    /// </summary>
    /// <typeparam name="TClientModelCreate">The client model to create an item.</typeparam>
    /// <typeparam name="TClientModel">The client model.</typeparam>
    /// <typeparam name="TClientId"></typeparam>
    /// <typeparam name="TServerModel">The server model.</typeparam>
    public interface ICrudModelMapper<in TClientModelCreate, TClientModel, in TClientId, TServerModel> : ICrdModelMapper<TClientModelCreate, TClientModel, TClientId, TServerModel>, IRudModelMapper<TClientModel, TServerModel>
    where TClientModel : TClientModelCreate
    {
    }
}
