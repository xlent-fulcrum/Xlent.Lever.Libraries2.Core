using System.Collections.Generic;
using System.Linq;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// A paging envelope for returning segments of data.
    /// </summary>
    public class PageEnvelope<TStorableItem, TId>
        where TStorableItem : IStorableItem<TId>
    {
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public PageEnvelope() { }

        /// <summary>
        /// Constructor for a page of data.
        /// </summary>
        /// <param name="offset">The offset for this page.</param>
        /// <param name="limit">The limit used for this page.</param>
        /// <param name="total">The total number of items including this page. Null if not known.</param>
        /// <param name="data">The data in this page.</param>
        public PageEnvelope(int offset, int limit, int? total, IEnumerable<TStorableItem> data)
        {
            var dataAsArray = data as TStorableItem[] ?? data.ToArray();
            PageInfo = new PageInfo
            {
                Offset = offset,
                Limit = limit,
                Total = total,
                Returned = dataAsArray.Length
            };
            Data = dataAsArray;
        }

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
