using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    /// <typeparam name="TManyModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IManyToOneComplete<TManyModel, TId> : IManyToOneComplete<TManyModel, TManyModel, TId>, ICrud<TManyModel, TId>
    {
    }

    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    /// <typeparam name="TManyModelCreate">The type for creating objects in persistant storage.</typeparam>
    /// <typeparam name="TManyModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IManyToOneComplete<in TManyModelCreate, TManyModel, TId> : ICrud<TManyModelCreate, TManyModel, TId>, IManyToOne<TManyModel, TId> 
        where TManyModel : TManyModelCreate
    {
        /// <summary>
        /// Delete all child items for a specific parent, <paramref name="parentId"/>.
        /// </summary>
        Task DeleteChildrenAsync(TId parentId, CancellationToken token = default(CancellationToken));
    }
}
