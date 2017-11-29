using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Test;

namespace Xlent.Lever.Libraries2.Core.Storage
{
    [TestClass]
    public class PageEnvelopeTest
    {
        private MemoryPersistance<PersonStorableItem<string>, string> _storage;

        [TestInitialize]
        public void Inititalize()
        {
            _storage = new MemoryPersistance<PersonStorableItem<string>, string>();
        }

        [TestMethod]
        public async Task TestForEachWithMultipleRead()
        {
            const int numberOfValues1 = 2;
            const int numberOfValues2 = 3;

            for (var i = 0; i < numberOfValues1; i++)
            {
                var person = new PersonStorableItem<string>
                {
                    Id = $"{i}",
                    GivenName = $"FirstName{i}",
                    Surname = $"LastName{i}"
                };
                await _storage.CreateAsync(person);
            }

            var values = new PageEnvelopeEnumerable<PersonStorableItem<string>>(offset => _storage.ReadAllAsync(offset, 1).Result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(numberOfValues1, values.Count());

            for (var i = 0; i < numberOfValues2; i++)
            {
                var person = new PersonStorableItem<string>
                {
                    Id = $"{i}",
                    GivenName = $"FirstName{i}",
                    Surname = $"LastName{i}"
                };
                await _storage.CreateAsync(person);
            }

            values = new PageEnvelopeEnumerable<PersonStorableItem<string>>(offset => _storage.ReadAllAsync(offset, 1).Result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(numberOfValues1+numberOfValues2, values.Count());
        }

        [TestMethod]
        public void TestEmptyData()
        {
            var values = new PageEnvelopeEnumerable<PersonStorableItem<string>>(offset => _storage.ReadAllAsync(offset, 1).Result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(values.Any());
        }

    }
}
