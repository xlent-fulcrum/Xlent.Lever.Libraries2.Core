using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <inheritdoc cref="IManyToOneCrud{TManyModelCreate, TManyModel,TId}" />
    public interface IManyToOneCrud<TManyModel, TId> : 
        IManyToOneCrud<TManyModel, TManyModel, TId>,
        IManyToOneCrd<TManyModel, TId>,
        ICrud<TManyModel, TId>
    {
    }

    /// <inheritdoc cref="IManyToOneCrd{TManyModelCreate, TManyModel,TId}" />
    public interface IManyToOneCrud<in TManyModelCreate, TManyModel, TId> :
        IManyToOneCrd<TManyModelCreate, TManyModel, TId>,
        IManyToOneRud<TManyModel, TId>,
        ICrud<TManyModelCreate, TManyModel, TId>
        where TManyModel : TManyModelCreate
    {
    }
}
