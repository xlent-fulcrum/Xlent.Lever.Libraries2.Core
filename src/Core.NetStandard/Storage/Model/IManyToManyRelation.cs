using System;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting groups of objects.
    /// </summary>
    public interface IManyToManyRelation<TOneModel1, TOneModel2, in TId>
    {
        /// <summary>
        /// Find all referenced items with foreign key 1 set to <paramref name="foreignKey1Id"/>.
        /// </summary>
        /// <param name="foreignKey1Id">The specific foreign key value to read the referenced items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<PageEnvelope<TOneModel2>> ReadReferencedItemsByForeignKey1(TId foreignKey1Id, int offset = 0, int? limit = null);

        /// <summary>
        /// Find all referenced items with foreign key 2 set to <paramref name="foreignKey2Id"/>.
        /// </summary>
        /// <param name="foreignKey2Id">The specific foreign key value to read the referenced items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<PageEnvelope<TOneModel1>> ReadReferencedItemsByForeignKey2(TId foreignKey2Id, int offset = 0, int? limit = null);

        /// <summary>
        /// Delete all references where foreign key 1 is set to <paramref name="foreignKey1Id"/>.
        /// </summary>
        Task DeleteReferencesByForeignKey1(TId foreignKey1Id);

        /// <summary>
        /// Delete all references where foreign key 2 is set to <paramref name="foreignKey2Id"/>.
        /// </summary>
        Task DeleteReferencesByForeignKey2(TId foreignKey2Id);
    }
}
