using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    internal class CacheEnvelope
    {
        public string CacheIdentity { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public byte[] Data { get; set; }
    }
}
