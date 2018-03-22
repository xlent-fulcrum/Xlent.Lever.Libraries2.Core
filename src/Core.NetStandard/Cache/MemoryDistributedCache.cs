using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Logic;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public class MemoryDistributedCache : IDistributedCache
    {
        /// <summary>
        /// The actual storage of the items.
        /// </summary>
        protected readonly MemoryPersistance<byte[], string> ItemStorage = new MemoryPersistance<byte[], string>();


        /// <inheritdoc />
        public byte[] Get(string key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNullOrWhitespace(key, nameof(key));
            return await ItemStorage.ReadAsync(key);
        }

        /// <inheritdoc />
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNullOrWhitespace(key, nameof(key));
            InternalContract.RequireNotNull(value, nameof(value));

            var item = await GetAsync(key, token);
            if (item == null)
            {
                try
                {
                    await ItemStorage.CreateWithSpecifiedIdAsync(key, value);
                }
                catch (FulcrumConflictException e)
                {
                    await ItemStorage.UpdateAsync(key, value);
                }
            }
            else
            {
                await ItemStorage.UpdateAsync(key, value);
            }
        }

        /// <inheritdoc />
        public void Refresh(string key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNullOrWhitespace(key, nameof(key));
            await ItemStorage.DeleteAsync(key);
        }
    }
}

