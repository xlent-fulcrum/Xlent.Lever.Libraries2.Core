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
    public class TestAutoCacheManyToOneRecursive : TestAutoCacheBase<ItemWithParentId>
    {
        private AutoCacheManyToOneRecursive<ItemWithParentId, Guid> _autoCache;

        /// <inheritdoc />
        public override AutoCacheRead<ItemWithParentId, Guid> AutoCacheRead => _autoCache;

        private IManyToOneRecursiveRelationComplete<ItemWithParentId, Guid> _storage;
        /// <inheritdoc />
        protected override ICrud<ItemWithParentId, Guid> CrudStorage => _storage;


        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestAutoCacheRead).FullName);
            _storage = new MemoryManyToOneRecursivePersistance<ItemWithParentId, Guid>(item => item.ParentId);
            Cache = new MemoryDistributedCache();
            DistributedCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1000)
            };
            AutoCacheOptions = new AutoCacheOptions
            {
                AbsoluteExpirationRelativeToNow = DistributedCacheOptions.AbsoluteExpirationRelativeToNow
            };
            _autoCache = new AutoCacheManyToOneRecursive<ItemWithParentId, Guid>(_storage, Cache, null, AutoCacheOptions);
        }

        [TestMethod]
        public async Task ReadParentAsync()
        {
            var parentId = Guid.NewGuid();
            var parent = new ItemWithParentId("ParentA");
            await PrepareStorageAndCacheAsync(parentId, parent, null);
            var childId = Guid.NewGuid();
            var child = new ItemWithParentId("ChildA", parentId);
            await PrepareStorageAndCacheAsync(childId, child, null);
            var readParent = await _autoCache.ReadParentAsync(childId);
            UT.Assert.IsNotNull(readParent);
            UT.Assert.AreEqual(parent, readParent);
            var updatedParent = new ItemWithParentId("ParentB");
            await _storage.UpdateAsync(parentId, updatedParent);
            await VerifyAsync(parentId, updatedParent, readParent);
            await VerifyAsync(childId, child, null, child);
        }
    }
}
