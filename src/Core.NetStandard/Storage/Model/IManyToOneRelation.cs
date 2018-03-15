using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting groups of objects.
    /// </summary>
    public interface IManyToOneRelation<TManyModel, TOneModel, in TId, in TReferenceId> : IManyToOneRecursiveRelation<TManyModel, TId, TReferenceId>
        where TManyModel : class
        where TReferenceId : TId
    {
        /// <summary>
        /// Read the parent for the child <paramref name="childId"/>.
        /// </summary>
        /// <param name="childId">The specific child to read the parent item for.</param>
        new Task<TOneModel> ReadParentAsync(TId childId);
    }
}
