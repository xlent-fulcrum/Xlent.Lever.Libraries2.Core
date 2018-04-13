using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Cache.Microsoft.Extensions.Caching.Distributed;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    /// <summary>
    /// An interface for producing caches with interface <see cref="IDistributedCache"/>
    /// </summary>
    public interface IDistributedCacheFactory
    {
        /// <summary>
        /// Create or get a distributed cache.
        /// </summary>
        /// <param name="key">The key to find the cache if it already exists.</param>
        Task<IDistributedCache> CreateOrGetDistributedCacheAsync(string key);
    }
}
