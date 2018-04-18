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
    public class TestAutoCacheManyToOneRecursive : TestAutoCacheBase<ItemWithParentId>
    {
        private ManyToOneAutoCacheComplete<ItemWithParentId, Guid> _autoCache;

        private IManyToOneRelationComplete<ItemWithParentId, Guid> _storage;
        /// <inheritdoc />
        protected override ICrud<ItemWithParentId, Guid> CrudStorage => _storage;
        
        /// <inheritdoc />
        public override ReadAutoCache<ItemWithParentId, Guid> ReadAutoCache => _autoCache;


        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestAutoCacheRead).FullName);
            _storage = new ManyToOneMemory<ItemWithParentId, Guid>(item => item.ParentId);
            Cache = new MemoryDistributedCache();
            DistributedCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1000)
            };
            AutoCacheOptions = new AutoCacheOptions
            {
                AbsoluteExpirationRelativeToNow = DistributedCacheOptions.AbsoluteExpirationRelativeToNow
            };
            _autoCache = new ManyToOneAutoCacheComplete<ItemWithParentId, Guid>(_storage, Cache, null, AutoCacheOptions);
        }

        [TestMethod]
        [Ignore] // The test fails when all tests are run for the solution, but not if only the tests for Cache is run!?!
        public async Task ReadChildren()
        {
            AutoCacheOptions.SaveCollections = true;
            AutoCacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            _autoCache = new ManyToOneAutoCacheComplete<ItemWithParentId, Guid>(_storage, Cache, null, AutoCacheOptions);
            var parentId = Guid.NewGuid();
            var parent = new ItemWithParentId(parentId, "ParentA");
            await PrepareStorageAndCacheAsync(parentId, parent, null);
            var childId1 = Guid.NewGuid();
            var child1A = new ItemWithParentId(childId1, "Child1A", parentId);
            await PrepareStorageAndCacheAsync(childId1, child1A, null);
            var childId2 = Guid.NewGuid();
            var child2A = new ItemWithParentId(childId2, "Child2A", parentId);
            await PrepareStorageAndCacheAsync(childId2, child2A, null);
            var result = await _autoCache.ReadChildrenAsync(parentId);
            UT.Assert.IsNotNull(result);
            var enumerable = result as ItemWithParentId[] ?? result.ToArray();
            UT.Assert.AreEqual(2, enumerable.Length);
            UT.Assert.IsTrue(enumerable.Contains(child1A));
            UT.Assert.IsTrue(enumerable.Contains(child2A));

            var child1B = new ItemWithParentId(childId1, "Child1B", parentId);
            await _storage.UpdateAsync(childId1, child1B);
            var child2B = new ItemWithParentId(childId2, "Child2B", parentId);
            await _storage.UpdateAsync(childId2, child2B);
            // Even though the items have been updated, the result will be fetched from the cache.
            result = await _autoCache.ReadChildrenAsync(parentId);
            UT.Assert.IsNotNull(result);
            enumerable = result as ItemWithParentId[] ?? result.ToArray();
            UT.Assert.AreEqual(2, enumerable.Length);
            UT.Assert.IsTrue(enumerable.Contains(child1A), $"Missing {child1A.Value} in " + string.Join(", ", enumerable.Select(item => item.Value)));
            UT.Assert.IsTrue(enumerable.Contains(child2A), $"Missing {child2A.Value} in " + string.Join(", ", enumerable.Select(item => item.Value)));
        }

        [TestMethod]
        public async Task ReadChildrenUpdatesIndividualItems()
        {
            AutoCacheOptions.SaveCollections = true;
            AutoCacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            _autoCache = new ManyToOneAutoCacheComplete<ItemWithParentId, Guid>(_storage, Cache, null, AutoCacheOptions);
            var parentId = Guid.NewGuid();
            var parent = new ItemWithParentId(parentId, "ParentA");
            await PrepareStorageAndCacheAsync(parentId, parent, null);
            var childId1 = Guid.NewGuid();
            var child1A = new ItemWithParentId(childId1, "Child1A", parentId);
            await PrepareStorageAndCacheAsync(childId1, child1A, null);
            var childId2 = Guid.NewGuid();
            var child2A = new ItemWithParentId(childId2, "Child2A", parentId);
            await PrepareStorageAndCacheAsync(childId2, child2A, null);
            var result = await _autoCache.ReadChildrenAsync(parentId);
            UT.Assert.IsNotNull(result);
            while (_autoCache.IsCollectionOperationActive(parentId)) await Task.Delay(TimeSpan.FromMilliseconds(10));
            await VerifyAsync(childId1, child1A);
            await VerifyAsync(childId2, child2A);
        }

        [TestMethod]
        public async Task DeleteChildren()
        {
            AutoCacheOptions.SaveCollections = true;
            AutoCacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            _autoCache = new ManyToOneAutoCacheComplete<ItemWithParentId, Guid>(_storage, Cache, null, AutoCacheOptions);
            var parentId = Guid.NewGuid();
            var parent = new ItemWithParentId(parentId, "ParentA");
            await PrepareStorageAndCacheAsync(parentId, parent, null);
            var childId1 = Guid.NewGuid();
            var child1A = new ItemWithParentId(childId1, "Child1A", parentId);
            await PrepareStorageAndCacheAsync(childId1, child1A, null);
            var childId2 = Guid.NewGuid();
            var child2A = new ItemWithParentId(childId2, "Child2A", parentId);
            await PrepareStorageAndCacheAsync(childId2, child2A, null);
            // Read into cache
            await _autoCache.ReadChildrenAsync(parentId);
            await _autoCache.DeleteChildrenAsync(parentId);
            while (_autoCache.IsCollectionOperationActive(parentId)) await Task.Delay(TimeSpan.FromMilliseconds(10));

            // Even though the items have been updated, the result will be fetched from the cache.
            var result = await _autoCache.ReadChildrenAsync(parentId);
            UT.Assert.IsNotNull(result);
            var enumerable = result as ItemWithParentId[] ?? result.ToArray();
            UT.Assert.AreEqual(0, enumerable.Length);
        }
    }
}
