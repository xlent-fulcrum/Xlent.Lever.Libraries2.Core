using System.Collections.Generic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// A paging envelope for returning segments of data.
    /// </summary>
    public class PageEnvelope<TStorableItem, TId>
        where TStorableItem : IStorableItem<TId>
    {
        /// <summary>
        /// The data in this segment, this "page"
        /// </summary>
        public PageInfo PageInfo { get; set; }

        /// <summary>
        /// Information about this segment of the data
        /// </summary>
        public IEnumerable<TStorableItem> Data { get; set; }
    }
}
