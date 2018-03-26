using System;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    public class CacheEnvelope
    {
        public string CacheIdentity { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public byte[] Data { get; set; }
    }
}
