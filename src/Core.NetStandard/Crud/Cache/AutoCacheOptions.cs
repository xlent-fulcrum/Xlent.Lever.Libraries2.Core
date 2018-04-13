using System;
using Microsoft.Extensions.Caching.Distributed;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Cache
{
    /// <summary>
    ///  Options that are rules for the <see cref="CrudAutoCache{TModel,TId}"/>.
    /// </summary>
    public class AutoCacheOptions
    {
        /// <summary>
        /// Is the strategy to save all items? If not, we will just save the items that we happen to stumble upon.
        /// </summary>
        public bool SaveAll { get; set; }

        /// <summary>
        /// When we just happen to have an Id to an item om the cache - should we get it from the storage to add it to the cache?
        /// </summary>
        public bool DoGetToUpdate { get; set; }

        /// <summary>
        /// When is an item considered too old? Will be used for <see cref="DistributedCacheEntryOptions.AbsoluteExpirationRelativeToNow"/>
        /// </summary>
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

        /// <summary>
        /// How often must the item be accessed to stay in the cache? Will be used for <see cref="DistributedCacheEntryOptions.SlidingExpiration"/>.
        /// </summary>
        public TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// When someone calls ReadAll, AutoCache will always save the individual items in the cache. If this property is true, then AutoCache will also save the entire array.
        /// </summary>
        public bool SaveCollections { get; set; }
    }
}
