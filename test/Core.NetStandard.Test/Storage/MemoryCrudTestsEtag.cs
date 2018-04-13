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
    public class MemoryCrudTestsEtag : TestICrudEtag<Guid>
    {
        private ICrud<TestItemEtag<Guid>, Guid> _storage;

        [TestInitialize]
        public void Inititalize()
        {
            _storage = new CrudMemory<TestItemEtag<Guid>, Guid>();
        }

        protected override ICrud<TestItemEtag<Guid>, Guid> CrudStorage => _storage;
    }
}
