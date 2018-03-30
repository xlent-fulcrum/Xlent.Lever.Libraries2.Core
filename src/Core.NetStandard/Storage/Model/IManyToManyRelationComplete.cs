﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    public interface IManyToManyRelationComplete<TManyToManyModel, TReferenceModel1, TReferenceModel2, TId> 
        : ICrud<TManyToManyModel, TId>, 
            IManyToManyRelation<TReferenceModel1, TReferenceModel2, TId>
    {
        /// <summary>
        /// Find all items with reference 1 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<PageEnvelope<TManyToManyModel>> ReadByReference1WithPagingAsync(TId id, int offset, int? limit = null);

        /// <summary>
        /// Find all items reference 1 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the items for.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<IEnumerable<TManyToManyModel>> ReadByReference1Async(TId id, int limit = int.MaxValue);

        /// <summary>
        /// Find all items with reference 2 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<PageEnvelope<TManyToManyModel>> ReadByReference2WithPagingAsync(TId id, int offset, int? limit = null);

        /// <summary>
        /// Find all items with reference 2 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to read the items for.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<IEnumerable<TManyToManyModel>> ReadByReference2Async(TId id, int limit = int.MaxValue);

        /// <summary>
        /// Delete all items reference 1 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to delete the items for.</param>
        Task DeleteByReference1Async(TId id);

        /// <summary>
        /// Delete all items reference 2 set to <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The specific foreign key value to delete the items for.</param>
        Task DeleteByReference2Async(TId id);
    }
}
