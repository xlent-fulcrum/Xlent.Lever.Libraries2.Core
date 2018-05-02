using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Crud.Cache
{
    /// <summary>
    /// A factory for creating new caches.
    /// </summary>
    public class DistributedCacheFactoryMemory : IDistributedCacheFactory
    {
        private readonly ICrd<DistributedCacheMemory, DistributedCacheMemory, string> _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storage"></param>
        public DistributedCacheFactoryMemory(ICrd<DistributedCacheMemory, DistributedCacheMemory, string> storage)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<IDistributedCache> GetOrCreateDistributedCacheAsync(string key)
        {
            try
            {
                return await _storage.ReadAsync(key);
            }
            catch (FulcrumNotFoundException)
            {
                var cache = new DistributedCacheMemory();
                await _storage.CreateWithSpecifiedIdAsync(key, cache);
                return cache;
            }
        }
    }
}
