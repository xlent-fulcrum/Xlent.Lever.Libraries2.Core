using System;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Cache
{
    /// <summary>
    /// Information that a <see cref="AutoCacheBase{TModel,TId}.UseCacheStrategyMethodAsync"/> can base its decision on.
    /// </summary>
    /// <typeparam name="TId">THe type for the unique identifier for the item.</typeparam>
    public class CachedItemInformation<TId>
    {
        /// <summary>
        /// The unique identifier for this item.
        /// </summary>
        public TId Id { get; set; }

        /// <summary>
        /// The time that the cached item was last updated.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
