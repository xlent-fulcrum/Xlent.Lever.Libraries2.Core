using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    [TestClass]
    public class TestAutoCache
    {
        private IDistributedCache _cache;

        private MemoryPersistance<string, Guid> _storage;
        private AutoCache<string, Guid> _autoCache;
        private DistributedCacheEntryOptions _distributedCacheOptions;
        private static readonly Guid BaseGuid = Guid.NewGuid();
        private static readonly string BaseGuidString = BaseGuid.ToString();
        private AutoCacheOptions _autoCacheOptions;

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
            _autoCacheOptions = new AutoCacheOptions
            {
                AbsoluteExpirationRelativeToNow = _distributedCacheOptions.AbsoluteExpirationRelativeToNow
            };
            _autoCache = new AutoCache<string, Guid>(_storage, FromStringToGuid, _cache, null, _autoCacheOptions);
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
            UT.Assert.IsNotNull(_autoCacheOptions.AbsoluteExpirationRelativeToNow);
            await Task.Delay(_autoCacheOptions.AbsoluteExpirationRelativeToNow.Value);
            await VerifyAsync(id, "B", "A", "B");

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
                _autoCacheOptions.DoGetToUpdate = doGetToUpdate;
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
            _autoCacheOptions.SaveAll = true;
            _autoCache = new AutoCache<string, Guid>(_storage, FromStringToGuid, _cache, null, _autoCacheOptions);
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
                _autoCacheOptions.DoGetToUpdate = doGetToUpdate;
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
            _autoCacheOptions.SaveAll = true;
            _autoCache = new AutoCache<string, Guid>(_storage, FromStringToGuid, _cache, null, _autoCacheOptions);
            var id = await _autoCache.CreateAsync("A"); // Will update cache thanks to SaveAll
            await PrepareStorageAsync(id, "B");
            await VerifyAsync(id, "B", "A");
        }

        [TestMethod]
        public async Task UpdateStorage_ReadNewCache()
        {
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", "A");
            await _autoCache.UpdateAndReturnAsync(id, "B");
            await PrepareStorageAsync(id, "C");
            await VerifyAsync(id, "C", "B");
        }

        [TestMethod]
        public async Task UpdateStorage_ReadOldCache()
        {
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", "A");
            await _autoCache.UpdateAsync(id, "B"); // Will not update cache
            await VerifyAsync(id, "B", "A");
        }

        [TestMethod]
        public async Task UpdateStorage_GetOption_ReadCache()
        {
            _autoCacheOptions.DoGetToUpdate = true;
            _autoCache = new AutoCache<string, Guid>(_storage, FromStringToGuid, _cache, null, _autoCacheOptions);
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", "A");
            await _autoCache.UpdateAsync(id, "B"); // Will update cache thanks to DoGetToUpdate
            await PrepareStorageAsync(id, "C");
            await VerifyAsync(id, "C", "B");
        }

        [TestMethod]
        public async Task UpdateStorage_SaveOption_ReadCache()
        {
            _autoCacheOptions.SaveAll = true;
            _autoCache = new AutoCache<string, Guid>(_storage, FromStringToGuid, _cache, null, _autoCacheOptions);
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", "A");
            await _autoCache.UpdateAsync(id, "B"); // Will update cache thanks to SaveAll
            await PrepareStorageAsync(id, "C");
            await VerifyAsync(id, "C", "B");
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
        public void TestFromStringToGuid()
        {
            // Not equal
            VerifyFromStringToGuidAreNotEqual("A", "B");
            VerifyFromStringToGuidAreNotEqual("A1", "A2");

            // Equal
            VerifyFromStringToGuidAreEqual("A1", "A2", 1);
        }

        private static void VerifyFromStringToGuidAreNotEqual(string a, string b, int maxLength = Int32.MaxValue)
        {
            var id1 = FromStringToGuid(a, maxLength);
            var id2 = FromStringToGuid(b, maxLength);
            UT.Assert.AreNotEqual(id1, id2);
        }
        private static void VerifyFromStringToGuidAreEqual(string a, string b, int maxLength = Int32.MaxValue)
        {
            var id1 = FromStringToGuid(a, maxLength);
            var id2 = FromStringToGuid(b, maxLength);
            UT.Assert.AreEqual(id1, id2);
        }

        [TestMethod]
        public async Task ReadAll()
        {
            _autoCacheOptions.SaveResultFromReadAll = true;
            _autoCacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            _autoCache = new AutoCache<string, Guid>(_storage, item => FromStringToGuid(item, 1), _cache, null, _autoCacheOptions);
            var id1 = FromStringToGuid("A1", 1);
            await PrepareStorageAndCacheAsync(id1, "A1", null);
            var id2 = FromStringToGuid("B1", 1);
            await PrepareStorageAndCacheAsync(id2, "B1", null);
            var result = await _autoCache.ReadAllAsync();
            UT.Assert.IsNotNull(result);
            while (_autoCache.SaveReadAllToCacheThreadIsActive) await Task.Delay(TimeSpan.FromMilliseconds(10));
            await VerifyAsync(id1, "A1");
            await VerifyAsync(id2, "B1");
            UT.Assert.IsNotNull(result);
            var enumerable = result as string[] ?? result.ToArray();
            UT.Assert.AreEqual(2, enumerable.Length);
            UT.Assert.IsTrue(enumerable.Contains("A1"));
            UT.Assert.IsTrue(enumerable.Contains("B1"));

            await PrepareStorageAsync(id1, "A2");
            await PrepareStorageAsync(id2, "B2");
            result = await _autoCache.ReadAllAsync();
            UT.Assert.IsNotNull(result);
            await VerifyAsync(id1, "A2", "A1");
            await VerifyAsync(id2, "B2", "B1");
            UT.Assert.IsNotNull(result);
            enumerable = result as string[] ?? result.ToArray();
            UT.Assert.AreEqual(2, enumerable.Length);
            UT.Assert.IsTrue(enumerable.Contains("A1"));
            UT.Assert.IsTrue(enumerable.Contains("B1"));
        }

        #region Support methods
        private async Task PrepareStorageAndCacheAsync(Guid id, string storageValue, string cacheValue)
        {
            await PrepareStorageAsync(id, storageValue);
            await PrepareCacheAsync(id, cacheValue);
        }

        private async Task PrepareCacheAsync(Guid id, string cacheValue)
        {
            if (cacheValue == null)
            {
                await _cache.RemoveAsync(id.ToString());
            }
            else
            {
                await _cache.SetAsync(id.ToString(), _autoCache.ToSerializedCacheEnvelope(cacheValue), _distributedCacheOptions);
            }
        }

        private async Task PrepareStorageAsync(Guid id, string storageValue)
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
        }

        private async Task VerifyAsync(Guid id, string expectedStorageValue, string expectedCacheValueBefore, string expectedReadValue, string expectedCacheValueAfter)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedCacheValueBefore, true);
            await VerifyRead(id, expectedReadValue);
            await VerifyCache(id, expectedCacheValueAfter, false);
        }

        private async Task VerifyAsync(Guid id, string expectedStorageValue, string expectedCacheValueBefore, string expectedReadValue)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedCacheValueBefore, true);
            await VerifyRead(id, expectedReadValue);
            await VerifyCache(id, expectedReadValue, false);
        }

        private async Task VerifyAsync(Guid id, string expectedStorageValue, string expectedCacheValueBefore)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedCacheValueBefore, true);
            await VerifyRead(id, expectedCacheValueBefore);
            await VerifyCache(id, expectedCacheValueBefore, false);
        }

        private async Task VerifyAsync(Guid id, string expectedStorageValue)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedStorageValue, true);
            await VerifyRead(id, expectedStorageValue);
            await VerifyCache(id, expectedStorageValue, false);
        }

        private async Task VerifyStorage(Guid id, string expectedStorageValue)
        {
            try
            {
                var actualStorageValue = await _storage.ReadAsync(id);
                UT.Assert.AreEqual(expectedStorageValue, actualStorageValue, "Storage verification failed.");
            }
            catch (FulcrumNotFoundException)
            {
                UT.Assert.IsNull(expectedStorageValue, $"Nothing found in storage, but expected \"{expectedStorageValue}\".");
            }
        }

        private async Task VerifyCache(Guid id, string expectedCacheValue, bool isBeforeRead)
        {
            var beforeOrAfter = isBeforeRead ? "before" : "after";
            var actualCacheSerializedValue = await _cache.GetAsync(id.ToString());
            if (actualCacheSerializedValue == null)
            {
                UT.Assert.IsNull(expectedCacheValue, $"Cache value was null, but expected \"{expectedCacheValue}\" {beforeOrAfter} read.");
            }
            else
            {
                var actualCacheValue = _autoCache.ToItem<string>(actualCacheSerializedValue);
                UT.Assert.AreEqual(expectedCacheValue, actualCacheValue, $"Cache verification {beforeOrAfter} read failed.");
            }
        }

        private async Task VerifyRead(Guid id, string expectedReadValue)
        {
            try
            {
                var actualReadValue = await _autoCache.ReadAsync(id);
                UT.Assert.AreEqual(expectedReadValue, actualReadValue, "AutoCache Read verification failed.");
            }
            catch (FulcrumNotFoundException)
            {
                UT.Assert.IsNull(expectedReadValue, $"Nothing found at read, but expected \"{expectedReadValue}\".");
            }
        }

        private static Guid FromStringToGuid(string item)
        {
            return FromStringToGuid(item, int.MaxValue);
        }

        private static Guid FromStringToGuid(string item, int maxLength)
        {
            if (item.Length > maxLength)
            {
                item = item.Substring(0, maxLength);
            }
            var itemAsInt = item.GetHashCode();
            var itemAsHex = itemAsInt.ToString("X");
            var itemLength = itemAsHex.Length;
            var totalLength = BaseGuidString.Length;
            var itemAsGuidString = BaseGuidString.Substring(0, totalLength - itemLength) + itemAsHex;
            return new Guid(itemAsGuidString);
        }

        #endregion
    }

    internal class ItemWithId : IUniquelyIdentifiable<Guid>
    {
        /// <inheritdoc />
        public Guid Id { get; set; }

        public string Value { get; set; }
    }
}
