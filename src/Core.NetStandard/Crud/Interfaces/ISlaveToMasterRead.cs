using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <inheritdoc cref="ISlaveToMaster{TModel,TId}" />
    public interface ISlaveToMasterRead<TModel, TId> :
        ISlaveToMaster<TModel, TId>, 
        IRead<TModel, SlaveToMasterId<TId>>
    {
    }
}
