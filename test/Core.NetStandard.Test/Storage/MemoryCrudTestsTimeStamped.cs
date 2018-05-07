using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.MemoryStorage;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Crud;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Storage
{
    [TestClass]
    public class MemoryCrudTestsTimeStamped : TestICrudTimeStamped<Guid>
    {
        private ICrud<TestItemBare, TestItemTimestamped<Guid>, Guid> _storage;

        [TestInitialize]
        public void Inititalize()
        {
            _storage = new CrudMemory<TestItemBare, TestItemTimestamped<Guid>, Guid>();
        }

        protected override ICrud<TestItemBare, TestItemTimestamped<Guid>, Guid> CrudStorage => _storage;
    }
}
