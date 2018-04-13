using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.MoveTo.Core.Crud.MemoryStorage;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public class DistributedCacheMemory : IDistributedCache, IFlushableCache
    {
        /// <summary>
        /// The actual storage of the items.
        /// </summary>
        protected readonly CrudMemory<byte[], string> ItemStorage = new CrudMemory<byte[], string>();


        /// <inheritdoc />
        public byte[] Get(string key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            try
            {
                InternalContract.RequireNotNullOrWhitespace(key, nameof(key));
                return await ItemStorage.ReadAsync(key, token);
            }
            catch (FulcrumNotFoundException)
            {
                return null;
            }
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
                    await ItemStorage.CreateWithSpecifiedIdAsync(key, value, token);
                }
                catch (FulcrumConflictException)
                {
                    await ItemStorage.UpdateAsync(key, value, token);
                }
            }
            else
            {
                await ItemStorage.UpdateAsync(key, value, token);
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
            await ItemStorage.DeleteAsync(key, token);
        }

        /// <inheritdoc />
        public async Task FlushAsync(CancellationToken token = default(CancellationToken))
        {
            await ItemStorage.DeleteAllAsync(token);
        }
    }
}

