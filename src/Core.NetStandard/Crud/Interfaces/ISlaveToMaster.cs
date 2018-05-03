using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Functionality for persisting objects that has no life of their own, but are only relevant with their master.
    /// Examples: A list of rows on an invoice, a list of attributes of an object, the contact details of a person.
    /// </summary>
    public interface ISlaveToMaster<TModel, TId> : ISlaveToMaster<TModel, TModel, TId>, ICreateSlave<TModel, TId>, ICreateSlaveWithSpecifiedId<TModel, TId>
    {
    }

    /// <summary>
    /// Functionality for persisting objects that has no life of their own, but are only relevant with their master.
    /// Examples: A list of rows on an invoice, a list of attributes of an object, the contact details of a person.
    /// </summary>
    public interface ISlaveToMaster<in TModelCreate, TModel, TId> :
        IManyToOne<TModel, TId>, ICreateSlave<TModelCreate, TModel, TId>, ICreateSlaveWithSpecifiedId<TModelCreate, TModel, TId>
        where TModel : TModelCreate
    {
        /// <summary>
        /// Delete all child items for a specific parent, <paramref name="masterId"/>.
        /// </summary>
        Task DeleteChildrenAsync(TId masterId, CancellationToken token = default(CancellationToken));
    }
}
