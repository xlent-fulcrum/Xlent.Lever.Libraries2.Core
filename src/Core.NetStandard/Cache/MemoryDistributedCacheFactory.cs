using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    /// <summary>
    /// A factory for creating new caches.
    /// </summary>
    public class MemoryDistributedCacheFactory : IDistributedCacheFactory
    {
        private readonly ICrd<MemoryDistributedCache, MemoryDistributedCache, string> _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storage"></param>
        public MemoryDistributedCacheFactory(ICrd<MemoryDistributedCache, MemoryDistributedCache, string> storage)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<IDistributedCache> CreateOrGetDistributedCacheAsync(string key)
        {
            var cache = await _storage.ReadAsync(key);

            if (cache != null) return cache;
            cache = new MemoryDistributedCache();
            await _storage.CreateWithSpecifiedIdAsync(key, cache);
            return cache;
        }
    }
}
