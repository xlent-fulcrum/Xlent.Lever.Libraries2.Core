using System.Collections.Generic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// A paging envelope for returning segments of data.
    /// </summary>
    public class PageEnvelope<TStorableItem, TId> : IPageEnvelope<TStorableItem, TId>
        where TStorableItem : IStorableItem<TId>
    {
        /// <inheritdoc />
        public PageInfo PageInfo { get; set; }

        /// <inheritdoc />
        public IEnumerable<TStorableItem> Data { get; set; }
    }
}
