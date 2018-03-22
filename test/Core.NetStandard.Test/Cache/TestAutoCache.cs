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

        private Mock<IDistributedCache> _cacheMock
            ;

        private MemoryPersistance<PersonStorableItem, Guid> _memoryPersistance;
        private AutoCache<PersonStorableItem, Guid> _autoCache;

        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestAutoCache).FullName);
            _memoryPersistance = new MemoryPersistance<PersonStorableItem, Guid>();
            _cacheMock = new Mock<IDistributedCache>();
            _cacheMock.Setup(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(), CancellationToken.None)).Returns(Task.FromResult((string)null));
            var options = new AutoCacheOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1000)

            };
            _autoCache = new AutoCache<PersonStorableItem, Guid>(_memoryPersistance, _cacheMock.Object, null, options);
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
            _autoCache.UseCacheStrategyMethodAsync = information => Task.FromResult(AutoCache<PersonStorableItem, Guid>.UseCacheStrategyEnum.Use);
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
            _cacheMock.Setup(cache => cache.GetAsync(storageOnlyId.ToString(), CancellationToken.None)).ReturnsAsync(() => null);
            return storageOnlyId;
        }

        private async Task<Guid> SetupCacheOnlyAsync()
        {
            var cachePerson = new PersonStorableItem("B", FromCache);
            var cacheOnlyId = Guid.NewGuid();
            _cacheMock.Setup(cache => cache.GetAsync(cacheOnlyId.ToString(), CancellationToken.None)).ReturnsAsync(() => _autoCache.ToSerializedCacheEnvelope(cachePerson));
            return await Task.FromResult(cacheOnlyId);
        }

        private async Task<Guid> SetupStorageAndCacheAsync()
        {
            var storagePerson2 = new PersonStorableItem("C", FromStorage);
            var cacheOrStorageId = Guid.NewGuid();
            await _memoryPersistance.CreateWithSpecifiedIdAsync(cacheOrStorageId, storagePerson2);
            var cachePerson2 = new PersonStorableItem("C", FromCache);
            var serializedCacheEnvelope = _autoCache.ToSerializedCacheEnvelope(cachePerson2);
            _cacheMock.Setup(cache => cache.GetAsync(cacheOrStorageId.ToString(), CancellationToken.None)).ReturnsAsync(() => serializedCacheEnvelope);
            return cacheOrStorageId;
        }
    }
}
