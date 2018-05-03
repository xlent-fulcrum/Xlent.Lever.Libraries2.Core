using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.MemoryStorage;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Storage
{
    [TestClass]
    public class PageEnvelopeTest
    {
        private ICrud<TestItemBare, TestItemId<string>, string> _storage;

        [TestInitialize]
        public void Inititalize()
        {
            _storage = new CrudMemory<TestItemBare, TestItemId<string>, string>();
        }

        [TestMethod]
        public async Task TestForEachWithMultipleRead()
        {
            const int numberOfValues1 = 2;
            const int numberOfValues2 = 3;

            for (var i = 0; i < numberOfValues1; i++)
            {
                var item = new TestItemId<string>
                {
                    Id = $"{i}",
                    Value = $"Value{i}"
                };
                await _storage.CreateAsync(item);
            }

            var values = new PageEnvelopeEnumerable<TestItemId<string>>((offset,t) => _storage.ReadAllWithPagingAsync(offset, 1, t).Result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(numberOfValues1, values.Count());

            for (var i = 0; i < numberOfValues2; i++)
            {
                var item = new TestItemId<string>
                {
                    Id = $"{i}",
                    Value = $"Value{i}"
                };
                await _storage.CreateAsync(item);
            }

            values = new PageEnvelopeEnumerable<TestItemId<string>>((offset, t) => _storage.ReadAllWithPagingAsync(offset, 1, t).Result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(numberOfValues1+numberOfValues2, values.Count());
        }

        [TestMethod]
        public void TestEmptyData()
        {
            var values = new PageEnvelopeEnumerable<TestItemId<string>>((offset, t) => _storage.ReadAllWithPagingAsync(offset, 1, t).Result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(values.Any());
        }

    }
}
