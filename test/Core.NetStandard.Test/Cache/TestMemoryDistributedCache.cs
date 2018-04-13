using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Cache.Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Crud.MemoryStorage;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    [TestClass]
    public class TestMemoryDistributedCache
    {
        private IDistributedCache _cache;

        [TestInitialize]
        public async Task Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestMemoryDistributedCache).FullName);
            var storage = new CrudMemory<MemoryDistributedCache, string>();
            var factory = new MemoryDistributedCacheFactory(storage);
            _cache = await factory.CreateOrGetDistributedCacheAsync(typeof(TestMemoryDistributedCache).FullName);
        }

        [TestMethod]
        public async Task GetNull()
        {
            var key = "1";
            var getValue = await _cache.GetAsync(key, CancellationToken.None);
            UT.Assert.IsNull(getValue);
        }

        [TestMethod]
        public async Task SetAndGet()
        {
            var key = "1";
            var data = "A";
            var value = Encoding.UTF8.GetBytes(data);
            await _cache.SetAsync(key, value, null, CancellationToken.None);
            var getValue = await _cache.GetAsync(key, CancellationToken.None);
            var getData = Encoding.UTF8.GetString(getValue);
            UT.Assert.AreEqual(data, getData);
        }

        [TestMethod]
        public async Task SetRemoveGet()
        {
            var key = "1";
            var data = "A";
            var value = Encoding.UTF8.GetBytes(data);
            await _cache.SetAsync(key, value, null, CancellationToken.None);
            await _cache.RemoveAsync(key, CancellationToken.None);
            var getValue = await _cache.GetAsync(key, CancellationToken.None);
            UT.Assert.IsNull(getValue);
        }
    }
}
