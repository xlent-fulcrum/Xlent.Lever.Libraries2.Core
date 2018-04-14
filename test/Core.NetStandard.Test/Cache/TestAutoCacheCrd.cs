using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Crud.Cache;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.MemoryStorage;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    [TestClass]
    public class TestAutoCacheCrd : TestAutoCacheBase<string>
    {
        private CrdAutoCache<string, Guid> _autoCache;

        /// <inheritdoc />
        public override ReadAutoCache<string, Guid> ReadAutoCache => _autoCache;

        private ICrud<string, Guid> _storage;
        /// <inheritdoc />
        protected override ICrud<string, Guid> CrudStorage => _storage;

        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestAutoCacheCrd).FullName);
            _storage = new CrudMemory<string, Guid>();
            Cache = new MemoryDistributedCache();
            DistributedCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1000)
            };
            AutoCacheOptions = new AutoCacheOptions
            {
                AbsoluteExpirationRelativeToNow = DistributedCacheOptions.AbsoluteExpirationRelativeToNow
            };
            _autoCache = new CrudAutoCache<string, Guid>(_storage, ToGuid, Cache, null, AutoCacheOptions);
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
            _autoCache = new CrudAutoCache<string, Guid>(_storage, ToGuid, Cache, null, AutoCacheOptions);
            var id = Guid.NewGuid();
            await _autoCache.CreateWithSpecifiedIdAsync(id, "A"); // Will update cache thanks to SaveAll
            await PrepareStorageAsync(id, "B");
            await VerifyAsync(id, "B", "A");
        }

        [TestMethod]
        public async Task CreateStorage_ReadCache()
        {
            await _autoCache.CreateAndReturnAsync("A");
            var id = ToGuid("A");
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
            _autoCache = new CrudAutoCache<string, Guid>(_storage, ToGuid, Cache, null, AutoCacheOptions);
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

        [TestMethod]
        public async Task DeleteAllRemovesCache()
        {
            AutoCacheOptions.SaveCollections = true;
            AutoCacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            _autoCache = new CrudAutoCache<string, Guid>(_storage, item => ToGuid(item, 1), Cache, null, AutoCacheOptions);
            var id1 = ToGuid("A1", 1);
            await PrepareStorageAndCacheAsync(id1, "A1", null);
            var id2 = ToGuid("B1", 1);
            await PrepareStorageAndCacheAsync(id2, "B1", null);
            await _autoCache.ReadAllAsync();
            await _autoCache.DeleteAllAsync();
            
            var result = await _autoCache.ReadAllAsync();
            UT.Assert.IsNotNull(result);
            var enumerable = result as string[] ?? result.ToArray();
            UT.Assert.AreEqual(0, enumerable.Length);
        }
    }
}
