using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <inheritdoc cref="ISlaveToMasterCrd{TModel,TId}" />
    public interface ISlaveToMasterCrd<TModel, TId> :
        ISlaveToMasterCrd<TModel, TModel, TId>,
        ICreateSlave<TModel, TId>,
        ICreateSlaveWithSpecifiedId<TModel, TId>
    {
    }

    /// <inheritdoc cref="ISlaveToMasterRead{TModel,TId}" />
    public interface ISlaveToMasterCrd<in TModelCreate, TModel, TId> :
        ISlaveToMasterRead<TModel, TId>,
        ICreateSlave<TModelCreate, TModel, TId>,
        ICreateSlaveWithSpecifiedId<TModelCreate, TModel, TId>,
        IDelete<SlaveToMasterId<TId>>
        where TModel : TModelCreate
    {
        /// <summary>
        /// Delete all child items for a specific parent, <paramref name="parentId"/>.
        /// </summary>
        Task DeleteChildrenAsync(TId parentId, CancellationToken token = default(CancellationToken));
    }
}
