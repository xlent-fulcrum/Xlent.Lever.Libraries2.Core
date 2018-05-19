using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <inheritdoc cref="IManyToOneRead{TManyModel,TId}" />
    public interface IManyToOneRud<TManyModel, in TId> :
        IManyToOneRead<TManyModel, TId>,
        IRud<TManyModel, TId>
    {
        /// <summary>
        /// Delete all the children of the parent with id <paramref name="parentId"/>.
        /// </summary>
        Task DeleteChildrenAsync(TId parentId, CancellationToken token = default(CancellationToken));
    }
}
