using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    [TestClass]
    public class TestAutoCacheCrd : TestAutoCacheBase
    {
        private AutoCacheCrd<string, Guid> _autoCache;

        /// <inheritdoc />
        public override AutoCacheRead<string, Guid> AutoCacheRead => _autoCache;

        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestAutoCacheCrd).FullName);
            Storage = new MemoryPersistance<string, Guid>();
            Cache = new MemoryDistributedCache();
            DistributedCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1000)
            };
            AutoCacheOptions = new AutoCacheOptions
            {
                AbsoluteExpirationRelativeToNow = DistributedCacheOptions.AbsoluteExpirationRelativeToNow
            };
            _autoCache = new AutoCacheCrud<string, Guid>(Storage, FromStringToGuid, Cache, null, AutoCacheOptions);
        }

        [TestMethod]
        public async Task CreateStorageWithId_ReadCache()
        {
            var id = Guid.NewGuid();
            await _autoCache.CreateWithSpecifiedIdAndReturnAsync(id, "A");
            await PrepareStorageAsync(id, "B");
            await VerifyAsync(id, "B", "A");
        }

        /// <summary>
        /// The DoGetToUpdate setting does not matter when creating items.
        /// </summary>
        [TestMethod]
        public async Task CreateStorageWithId_ReadStorage()
        {
            var doGetToUpdate = false;
            while (true)
            {
                AutoCacheOptions.DoGetToUpdate = doGetToUpdate;
                var id = Guid.NewGuid();
                await _autoCache.CreateWithSpecifiedIdAsync(id, "A"); // Will not update cache
                await PrepareStorageAsync(id, "B");
                await VerifyAsync(id, "B", null, "B");
                if (doGetToUpdate) break;
                doGetToUpdate = true;
            }
        }

        [TestMethod]
        public async Task CreateStorageWithIdWith_SaveOption_ReadCache()
        {
            AutoCacheOptions.SaveAll = true;
            _autoCache = new AutoCacheCrud<string, Guid>(Storage, FromStringToGuid, Cache, null, AutoCacheOptions);
            var id = Guid.NewGuid();
            await _autoCache.CreateWithSpecifiedIdAsync(id, "A"); // Will update cache thanks to SaveAll
            await PrepareStorageAsync(id, "B");
            await VerifyAsync(id, "B", "A");
        }

        [TestMethod]
        public async Task CreateStorage_ReadCache()
        {
            await _autoCache.CreateAndReturnAsync("A");
            var id = FromStringToGuid("A");
            await PrepareStorageAsync(id, "B");
            await VerifyAsync(id, "B", "A");
        }

        /// <summary>
        /// The DoGetToUpdate setting does not matter when creating items.
        /// </summary>

        [TestMethod]
        public async Task CreateStorage_ReadStorage()
        {
            var doGetToUpdate = false;
            while (true)
            {
                AutoCacheOptions.DoGetToUpdate = doGetToUpdate;
                var id = await _autoCache.CreateAsync("A"); // Will not update cache
                await PrepareStorageAsync(id, "B");
                await VerifyAsync(id, "B", null, "B");
                if (doGetToUpdate) break;
                doGetToUpdate = true;
            }
        }

        [TestMethod]
        public async Task CreateStorage_SaveOption_ReadCache()
        {
            AutoCacheOptions.SaveAll = true;
            _autoCache = new AutoCacheCrud<string, Guid>(Storage, FromStringToGuid, Cache, null, AutoCacheOptions);
            var id = await _autoCache.CreateAsync("A"); // Will update cache thanks to SaveAll
            await PrepareStorageAsync(id, "B");
            await VerifyAsync(id, "B", "A");
        }

        [TestMethod]
        public async Task DeleteRemovesCache()
        {
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", "A");
            await _autoCache.DeleteAsync(id);
            await VerifyAsync(id, null);
        }
    }
}
