using System;
using System.Collections.Generic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use PageEnvelope instead.", true)]
    public interface IPageEnvelope<TData, TId>
        where TData : IStorableItem<TId>
    {
        /// <summary>
        /// The data in this segment, this "page"
        /// </summary>
        IEnumerable<TData> Data { get; set; }

        /// <summary>
        /// Information about this segment of the data
        /// </summary>
        PageInfo PageInfo { get; set; }
    }
}
