using Xlent.Lever.Libraries2.Core.Crud.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{

    /// <inheritdoc cref="ISlaveToMasterCrud{TModelCreate,TModel,TId}" />
    public interface ISlaveToMasterCrud<TModel, TId> :
        ISlaveToMasterCrud<TModel, TModel, TId>,
        ISlaveToMasterCrd<TModel, TId>
    {
    }

    /// <inheritdoc cref="ISlaveToMasterCrd{TModelCreate,TModel,TId}" />
    public interface ISlaveToMasterCrud<in TModelCreate, TModel, TId> :
        ISlaveToMasterCrd<TModelCreate, TModel, TId>,
        IRud<TModel, SlaveToMasterId<TId>>
        where TModel : TModelCreate
    {
    }
}
