using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;
using Xlent.Lever.Libraries2.MoveTo.Core.Crud.MemoryStorage;

namespace Xlent.Lever.Libraries2.Core.Storage
{
    [TestClass]
    public class MemoryCrudTestsId : TestICrudId<Guid>
    {
        private ICrud<TestItemId<Guid>, Guid> _storage;

        [TestInitialize]
        public void Inititalize()
        {
            _storage = new CrudMemory<TestItemId<Guid>, Guid>();
        }

        protected override ICrud<TestItemId<Guid>, Guid> CrudStorage => _storage;
    }
}
