using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    [TestClass]
    public class TestAutoCacheRead : TestAutoCacheBase<string>
    {
        private AutoCacheRead<string, Guid> _autoCache;

        /// <inheritdoc />
        public override AutoCacheRead<string, Guid> AutoCacheRead => _autoCache;

        private ICrud<string, Guid> _storage;
        /// <inheritdoc />
        protected override ICrud<string, Guid> CrudStorage => _storage;


        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestAutoCacheRead).FullName);
            _storage = new MemoryPersistance<string, Guid>();
            Cache = new MemoryDistributedCache();
            DistributedCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1000)
            };
            AutoCacheOptions = new AutoCacheOptions
            {
                AbsoluteExpirationRelativeToNow = DistributedCacheOptions.AbsoluteExpirationRelativeToNow
            };
            _autoCache = new AutoCacheRead<string, Guid>(_storage, ToGuid, Cache, AutoCacheOptions);
        }

        [TestMethod]
        public async Task ReadStorage()
        {
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", null);
            await VerifyAsync(id, "A", null, "A");
        }

        [TestMethod]
        public async Task ReadStorage_ReadCache()
        {
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", null);
            await _autoCache.ReadAsync(id);
            await VerifyAsync(id, "A");
        }

        [TestMethod]
        public async Task ReadStorage_MethodPrevent_ReadStorage()
        {
            _autoCache.UseCacheAtAllMethodAsync = type => Task.FromResult(false);
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", "A");
            await PrepareStorageAsync(id, "B");
            await VerifyAsync(id, "B", "A", "B");
        }

        [TestMethod]
        public async Task ReadStorage_MethodIgnore_ReadStorage()
        {
            _autoCache.UseCacheStrategyMethodAsync = type => Task.FromResult(UseCacheStrategyEnum.Ignore);
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", null);
            await VerifyAsync(id, "A", null, "A");
            await PrepareStorageAsync(id, "B");
            await VerifyAsync(id, "B", "A", "B");
        }

        [TestMethod]
        public async Task ReadStorage_MethodRemove_ReadStorage()
        {
            _autoCache.UseCacheStrategyMethodAsync = type => Task.FromResult(UseCacheStrategyEnum.Remove);
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", null);
            await VerifyAsync(id, "A", null, "A");
            await PrepareStorageAsync(id, "B");
            await VerifyAsync(id, "B", "A", "B");
        }

        [TestMethod]
        public async Task ReadStorage_ReadCacheTooLongDelay_ReadStorage()
        {
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", null);
            await VerifyAsync(id, "A", null, "A");
            await PrepareStorageAsync(id, "B");
            UT.Assert.IsNotNull(AutoCacheOptions.AbsoluteExpirationRelativeToNow);
            await Task.Delay(AutoCacheOptions.AbsoluteExpirationRelativeToNow.Value);
            await VerifyAsync(id, "B", "A", "B");

        }

        [TestMethod]
        public void TestFromStringToGuid()
        {
            // Not equal
            VerifyFromStringToGuidAreNotEqual("A", "B");
            VerifyFromStringToGuidAreNotEqual("A1", "A2");

            // Equal
            VerifyFromStringToGuidAreEqual("A1", "A2", 1);
        }

        private void VerifyFromStringToGuidAreNotEqual(string a, string b, int maxLength = Int32.MaxValue)
        {
            var id1 = ToGuid(a, maxLength);
            var id2 = ToGuid(b, maxLength);
            UT.Assert.AreNotEqual(id1, id2);
        }
        private void VerifyFromStringToGuidAreEqual(string a, string b, int maxLength = Int32.MaxValue)
        {
            var id1 = ToGuid(a, maxLength);
            var id2 = ToGuid(b, maxLength);
            UT.Assert.AreEqual(id1, id2);
        }

        [TestMethod]
        public async Task ReadAll()
        {
            AutoCacheOptions.SaveCollections = true;
            AutoCacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            _autoCache = new AutoCacheCrud<string, Guid>(_storage, item => ToGuid(item, 1), Cache, null, AutoCacheOptions);
            var id1 = ToGuid("A1", 1);
            await PrepareStorageAndCacheAsync(id1, "A1", null);
            var id2 = ToGuid("B1", 1);
            await PrepareStorageAndCacheAsync(id2, "B1", null);
            var result = await _autoCache.ReadAllAsync();
            UT.Assert.IsNotNull(result);
            var enumerable = result as string[] ?? result.ToArray();
            UT.Assert.AreEqual(2, enumerable.Length);
            UT.Assert.IsTrue(enumerable.Contains("A1"));
            UT.Assert.IsTrue(enumerable.Contains("B1"));

            await _storage.UpdateAsync(id1, "A2");
            await _storage.UpdateAsync(id2, "B2");
            // Even though the items have been updated, the result will be fetched from the cache.
            result = await _autoCache.ReadAllAsync();
            UT.Assert.IsNotNull(result);
            enumerable = result as string[] ?? result.ToArray();
            UT.Assert.AreEqual(2, enumerable.Length);
            UT.Assert.IsTrue(enumerable.Contains("A1"));
            UT.Assert.IsTrue(enumerable.Contains("B1"));
        }

        [TestMethod]
        public async Task ReadAllUpdatesIndividualItems()
        {
            AutoCacheOptions.SaveCollections = true;
            AutoCacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            _autoCache = new AutoCacheCrud<string, Guid>(_storage, item => ToGuid(item, 1), Cache, null, AutoCacheOptions);
            var id1 = ToGuid("A1", 1);
            await PrepareStorageAndCacheAsync(id1, "A1", null);
            var id2 = ToGuid("B1", 1);
            await PrepareStorageAndCacheAsync(id2, "B1", null);
            var result = await _autoCache.ReadAllAsync();
            UT.Assert.IsNotNull(result);
            while (_autoCache.GetSaveReadAllToCacheThreadIsActive()) await Task.Delay(TimeSpan.FromMilliseconds(10));
            await VerifyAsync(id1, "A1");
            await VerifyAsync(id2, "B1");
        }
    }
}
