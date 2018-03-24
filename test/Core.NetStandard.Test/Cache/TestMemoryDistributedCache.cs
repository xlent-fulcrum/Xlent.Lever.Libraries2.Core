using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    [TestClass]
    public class TestMemoryDistributedCache
    {
        private IDistributedCache _cache;

        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestAutoCache).FullName);
            _cache = new MemoryDistributedCache();
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
