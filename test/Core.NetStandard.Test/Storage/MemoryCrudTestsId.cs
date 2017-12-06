using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Storage
{
    [TestClass]
    public class MemoryCrudTestsId : TestICrudId<Guid>
    {
        private MemoryPersistance<TestItemId<Guid>, Guid> _storage;

        [TestInitialize]
        public void Inititalize()
        {
            _storage = new MemoryPersistance<TestItemId<Guid>, Guid>();
        }

        protected override ICrud<TestItemId<Guid>, Guid> CrudStorage => _storage;
    }
}
