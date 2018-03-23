using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Application;
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

        private MemoryPersistance<PersonStorableItem, Guid> _memoryPersistance;
        private AutoCache<PersonStorableItem, Guid> _autoCache;

        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestAutoCache).FullName);
            _memoryPersistance = new MemoryPersistance<PersonStorableItem, Guid>();
            _cache = new MemoryDistributedCache();
            var options = new AutoCacheOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1000)

            };
            _autoCache = new AutoCache<PersonStorableItem, Guid>(_memoryPersistance, _cache, null, options);
        }

        [TestMethod]
        public async Task FromStorageTest()
        {
            var storageOnlyId = await SetupStorageOnlyAsync();
            var person = await _autoCache.ReadAsync(storageOnlyId);
            UT.Assert.AreEqual("A", person.GivenName);
            UT.Assert.AreEqual(FromStorage, person.Surname);
        }

        [TestMethod]
        public async Task FromCacheTest()
        {
            var cacheOnlyId = await SetupCacheOnlyAsync();
            var person = await _autoCache.ReadAsync(cacheOnlyId);
            UT.Assert.AreEqual("B", person.GivenName);
            UT.Assert.AreEqual(FromCache, person.Surname);
        }

        [TestMethod]
        public async Task CacheFirstThenStorageAfterTimeOut()
        {
            var cacheOrStorageId = await SetupStorageAndCacheAsync();
            var person = await _autoCache.ReadAsync(cacheOrStorageId);
            UT.Assert.AreEqual("C", person.GivenName);
            UT.Assert.AreEqual(FromCache, person.Surname);
            await Task.Delay(TimeSpan.FromMilliseconds(1000));
            person = await _autoCache.ReadAsync(cacheOrStorageId);
            UT.Assert.AreEqual("C", person.GivenName);
            UT.Assert.AreEqual(FromStorage, person.Surname);
        }

        [TestMethod]
        public async Task DoNotUseCacheAtAll()
        {
            _autoCache.UseCacheAtAllMethodAsync = type => Task.FromResult(false);
            var cacheOrStorageId = await SetupStorageAndCacheAsync();
            var person = await _autoCache.ReadAsync(cacheOrStorageId);
            UT.Assert.AreEqual("C", person.GivenName);
            UT.Assert.AreEqual(FromStorage, person.Surname);
        }

        [TestMethod]
        public async Task UseCache()
        {
            var cacheOrStorageId = await SetupStorageAndCacheAsync();
            _autoCache.UseCacheStrategyMethodAsync = information => Task.FromResult(AutoCache<PersonStorableItem, Guid>.UseCacheStrategyEnum.Use);
            var person = await _autoCache.ReadAsync(cacheOrStorageId);
            UT.Assert.AreEqual("C", person.GivenName);
            UT.Assert.AreEqual(FromCache, person.Surname);
        }

        [TestMethod]
        public async Task IgnoreCacheThenUse()
        {
            var cacheOrStorageId = await SetupStorageAndCacheAsync();
            _autoCache.UseCacheStrategyMethodAsync = information => Task.FromResult(AutoCache<PersonStorableItem, Guid>.UseCacheStrategyEnum.Ignore);
            var person = await _autoCache.ReadAsync(cacheOrStorageId);
            UT.Assert.AreEqual("C", person.GivenName);
            UT.Assert.AreEqual(FromStorage, person.Surname);
            _autoCache.UseCacheStrategyMethodAsync = information =>
            {
                return Task.FromResult(AutoCache<PersonStorableItem, Guid>.UseCacheStrategyEnum.Use);
            };
            person = await _autoCache.ReadAsync(cacheOrStorageId);
            UT.Assert.AreEqual("C", person.GivenName);
            UT.Assert.AreEqual(FromCache, person.Surname);
        }

        [TestMethod]
        public async Task RemoveCacheThenUse()
        {
            var cacheOrStorageId = await SetupStorageAndCacheAsync();
            _autoCache.UseCacheStrategyMethodAsync = information => Task.FromResult(AutoCache<PersonStorableItem, Guid>.UseCacheStrategyEnum.Remove);
            var person = await _autoCache.ReadAsync(cacheOrStorageId);
            UT.Assert.AreEqual("C", person.GivenName);
            UT.Assert.AreEqual(FromStorage, person.Surname);
            _autoCache.UseCacheStrategyMethodAsync = information => Task.FromResult(AutoCache<PersonStorableItem, Guid>.UseCacheStrategyEnum.Use);
            person = await _autoCache.ReadAsync(cacheOrStorageId);
            UT.Assert.AreEqual("C", person.GivenName);
            UT.Assert.AreEqual(FromStorage, person.Surname);
        }

        private async Task<Guid> SetupStorageOnlyAsync()
        {
            var storagePerson = new PersonStorableItem("A", FromStorage);
            var storageOnlyId = Guid.NewGuid();
            await _memoryPersistance.CreateWithSpecifiedIdAsync(storageOnlyId, storagePerson);
            return storageOnlyId;
        }

        private async Task<Guid> SetupCacheOnlyAsync()
        {
            var cachePerson = new PersonStorableItem("B", FromCache);
            var cacheOnlyId = Guid.NewGuid();
            var serializedCacheEnvelope = _autoCache.ToSerializedCacheEnvelope(cachePerson);
            await _cache.SetAsync(cacheOnlyId.ToString(), serializedCacheEnvelope, null, CancellationToken.None);
            return await Task.FromResult(cacheOnlyId);
        }

        private async Task<Guid> SetupStorageAndCacheAsync()
        {
            var cacheOrStorageId = Guid.NewGuid();
            var cachePerson = new PersonStorableItem("C", FromCache);
            await _autoCache.CreateWithSpecifiedIdAsync(cacheOrStorageId, cachePerson);
            var storagePerson = await _memoryPersistance.ReadAsync(cacheOrStorageId);
            await _memoryPersistance.UpdateAsync(cacheOrStorageId, storagePerson);
            return cacheOrStorageId;
        }
    }
}
