using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Test;

namespace Xlent.Lever.Libraries2.Core.Storage
{
    [TestClass]
    public class MemoryStorageTest
    {
        private MemoryStorage<PersonStorableItem<Guid>, Guid> _storage;
        private StorageTestCrud<MemoryStorage<PersonStorableItem<Guid>, Guid>, PersonStorableItem<Guid>, Guid> _testCrud;

        [TestInitialize]
        public void Inititalize()
        {
            _storage = new MemoryStorage<PersonStorableItem<Guid>, Guid>();
            _testCrud = new StorageTestCrud<MemoryStorage<PersonStorableItem<Guid>, Guid>, PersonStorableItem<Guid>, Guid>(_storage);
        }

        [TestMethod]
        public async Task TestCrud()
        {
            await _testCrud.RunAllTests();
        }
        
    }
}
