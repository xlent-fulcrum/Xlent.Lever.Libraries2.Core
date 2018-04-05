using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    public interface IManyToManyRelation<TReferenceModel1, TReferenceModel2, in TId>
    {
        /// <summary>
        /// Find all referenced items with foreign key 1 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the referenced items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<PageEnvelope<TReferenceModel2>> ReadReferencedItemsByReference1WithPagingAsync(TId id, int offset, int? limit = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Find all referenced items with foreign key 1 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the referenced items for.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<IEnumerable<TReferenceModel2>> ReadReferencedItemsByReference1Async(TId id, int limit = int.MaxValue, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Find all referenced items with foreign key 2 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the referenced items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<PageEnvelope<TReferenceModel1>> ReadReferencedItemsByReference2WithPagingAsync(TId id, int offset, int? limit = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Find all referenced items with foreign key 2 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the referenced items for.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        Task<IEnumerable<TReferenceModel1>> ReadReferencedItemsByReference2Async(TId id, int limit = int.MaxValue, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Delete all references where foreign key 1 is set to <paramref name="id"/>.
        /// </summary>
        Task DeleteReferencesByReference1(TId id, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Delete all references where foreign key 2 is set to <paramref name="id"/>.
        /// </summary>
        Task DeleteReferencesByReference2(TId id, CancellationToken token = default(CancellationToken));
    }
}
