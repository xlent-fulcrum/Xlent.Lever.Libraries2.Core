using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Support;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    [TestClass]
    public class TestAutoCache
    {
        private const string FromStorage = "FromStorage";
        private const string FromCache = "FromCache";

        private IDistributedCache _cache;

        private MemoryPersistance<string, Guid> _storage;
        private AutoCache<string, Guid> _autoCache;
        private DistributedCacheEntryOptions _distributedCacheOptions;

        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestAutoCache).FullName);
            _storage = new MemoryPersistance<string, Guid>();
            _cache = new MemoryDistributedCache();
            _distributedCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1000)
            };
            var options = new AutoCacheOptions
            {
                AbsoluteExpirationRelativeToNow = _distributedCacheOptions.AbsoluteExpirationRelativeToNow

            };
            _autoCache = new AutoCache<string, Guid>(_storage, item =>
            {
                FulcrumAssert.Fail("This method should never be called, because we always provide an id");
                return Guid.Empty;
            }, _cache, null, options);
        }

        [TestMethod]
        public async Task ReadWithNoCache()
        {
            var id = Guid.NewGuid();
            // Objective: Storage=A Cache=null
            await PrepareStorageAndCache(id, "A", null);
            var value = await _autoCache.ReadAsync(id);
            UT.Assert.AreEqual("A", value);
        }

        private async Task PrepareStorageAndCache(Guid id, string storageValue, string cacheValue)
        {
            if (storageValue == null)
            {
                await _storage.DeleteAsync(id);
            }
            else
            {
                try
                {
                    var value = await _storage.ReadAsync(id);
                    if (value != storageValue) await _storage.UpdateAsync(id, storageValue);
                }
                catch (FulcrumNotFoundException)
                {
                    await _storage.CreateWithSpecifiedIdAsync(id, storageValue);
                }
            }
            if (cacheValue == null)
            {
                await _cache.RemoveAsync(id.ToString());
            }
            else
            {
                await _cache.SetAsync(id.ToString(), Encoding.UTF8.GetBytes(cacheValue), _distributedCacheOptions);
            }
        }
    }
}
