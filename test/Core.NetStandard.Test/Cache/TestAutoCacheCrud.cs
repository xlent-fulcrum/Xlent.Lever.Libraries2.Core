using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.MoveTo.Core.Crud.MemoryStorage;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    [TestClass]
    public class TestAutoCacheCrud : TestAutoCacheBase<string>
    {
        private CrudAutoCache<string, Guid> _autoCache;

        /// <inheritdoc />
        public override ReadAutoCache<string, Guid> ReadAutoCache => _autoCache;

        private ICrud<string, Guid> _storage;
        /// <inheritdoc />
        protected override ICrud<string, Guid> CrudStorage => _storage;

        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestAutoCacheCrud).FullName);
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
            AutoCacheOptions.DoGetToUpdate = true;
            _autoCache = new CrudAutoCache<string, Guid>(_storage, ToGuid, Cache, null, AutoCacheOptions);
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", "A");
            await _autoCache.UpdateAsync(id, "B"); // Will update cache thanks to DoGetToUpdate
            await PrepareStorageAsync(id, "C");
            await VerifyAsync(id, "C", "B");
        }

        [TestMethod]
        public async Task UpdateStorage_SaveOption_ReadCache()
        {
            AutoCacheOptions.SaveAll = true;
            _autoCache = new CrudAutoCache<string, Guid>(_storage, ToGuid, Cache, null, AutoCacheOptions);
            var id = Guid.NewGuid();
            await PrepareStorageAndCacheAsync(id, "A", "A");
            await _autoCache.UpdateAsync(id, "B"); // Will update cache thanks to SaveAll
            await PrepareStorageAsync(id, "C");
            await VerifyAsync(id, "C", "B");
        }
    }
}
