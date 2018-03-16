using System;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    internal class CacheEnvelope
    {
        public string CacheIdentity { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public byte[] Data { get; set; }
    }
}
