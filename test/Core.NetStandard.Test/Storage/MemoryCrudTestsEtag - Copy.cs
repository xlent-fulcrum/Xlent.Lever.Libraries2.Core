using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Storage
{
    [TestClass]
    public class MemoryCrudTestsTimeStamped : TestICrudTimeStamped<Guid>
    {
        private MemoryPersistance<TestItemTimestamped, Guid> _storage;

        [TestInitialize]
        public void Inititalize()
        {
            _storage = new MemoryPersistance<TestItemTimestamped, Guid>();
        }

        protected override ICrud<TestItemTimestamped, Guid> CrudStorage => _storage;
    }
}
