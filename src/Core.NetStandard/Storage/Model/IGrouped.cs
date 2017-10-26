﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting groups of objects.
    /// </summary>
    public interface IGrouped<TStorableItem, TId, in TGroup>
        where TStorableItem : IStorableItem<TId>
    {
        /// <summary>
        /// Create a new <paramref name="item"/> in the group <paramref name="groupValue"/>.
        /// </summary>
        /// <returns></returns>
        Task<TStorableItem> CreateAsync(TGroup groupValue, TStorableItem item);

        /// <summary>
        /// Read all items for a specific group, <paramref name="groupValue"/>.
        /// </summary>
        /// <param name="groupValue">The specific group to read the items for.</param>
        /// <param name="offset">The number of items that will be skipped in result.</param>
        /// <param name="limit">The maximum number of items to return.</param>
        Task<IPageEnvelope<TStorableItem, TId>> ReadAllAsync(TGroup groupValue, int offset = 0, int? limit = null);

        /// <summary>
        /// Delete all items for a specific group, <paramref name="groupValue"/>.
        /// </summary>
        Task DeleteAllAsync(TGroup groupValue);
    }
}
