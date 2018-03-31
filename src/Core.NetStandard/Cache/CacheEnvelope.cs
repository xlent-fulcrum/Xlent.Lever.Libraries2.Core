using System;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    /// <summary>
    /// Meta data for a thing stored to cache
    /// </summary>
    public class CacheEnvelope
    {
        /// <summary>
        /// The cache that the item was stored in.
        /// </summary>
        public string CacheIdentity { get; set; }
        /// <summary>
        /// When the item was last updated.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }
        /// <summary>
        /// The item, serialized into a byte array.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
