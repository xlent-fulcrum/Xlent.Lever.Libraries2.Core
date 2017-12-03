﻿using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Update an item of type <see cref="IIdentifiable{TId}"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of objects to update in persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the id parameter.</typeparam>
    public interface IUpdate<TItem, in TId>
    {
        /// <summary>
        /// Updates the item uniquely identified by <paramref name="item.Id"/> in storage.
        /// </summary>
        /// <param name="id">How the object to be updated is identified.</param>
        /// <param name="item">The new version of the item.</param>
        /// <exception cref="FulcrumNotFoundException">Thrown if the <paramref name="id"/> could not be found.</exception>
        /// <exception cref="FulcrumConflictException">Thrown if the <see cref="IOptimisticConcurrencyControlByETag.ETag"/> for <paramref name="item"/> was outdated.</exception>
        Task UpdateAsync(TId id, TItem item);

        /// <summary>
        /// Updates the item uniquely identified by <paramref name="item.Id"/> in storage.
        /// </summary>
        /// <param name="id">How the object to be updated is identified.</param>
        /// <param name="item">The new version of the item. </param>
        /// <returns>The updated item as it was saved.</returns>
        /// <exception cref="FulcrumNotFoundException">Thrown if the <paramref name="id"/> could not be found.</exception>
        /// <exception cref="FulcrumConflictException">Thrown if the <see cref="IOptimisticConcurrencyControlByETag.ETag"/> for <paramref name="item"/> was outdated.</exception>
        Task<TItem> UpdateAndReturnAsync(TId id, TItem item);
    }
}
