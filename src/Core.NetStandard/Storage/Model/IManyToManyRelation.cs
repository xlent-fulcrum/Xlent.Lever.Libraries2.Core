using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting groups of objects.
    /// </summary>
    public interface IManyToManyRelation<TReferenceModel1, TReferenceModel2, in TId>
    {
        /// <summary>
        /// Find all referenced items with foreign key 1 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the referenced items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<PageEnvelope<TReferenceModel2>> ReadReferencedItemsByForeignKey1(TId id, int offset = 0, int? limit = null);

        /// <summary>
        /// Find all referenced items with foreign key 2 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the referenced items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<PageEnvelope<TReferenceModel1>> ReadReferencedItemsByForeignKey2(TId id, int offset = 0, int? limit = null);

        /// <summary>
        /// Delete all references where foreign key 1 is set to <paramref name="id"/>.
        /// </summary>
        Task DeleteReferencesByForeignKey1(TId id);

        /// <summary>
        /// Delete all references where foreign key 2 is set to <paramref name="id"/>.
        /// </summary>
        Task DeleteReferencesByForeignKey2(TId id);
    }
}
