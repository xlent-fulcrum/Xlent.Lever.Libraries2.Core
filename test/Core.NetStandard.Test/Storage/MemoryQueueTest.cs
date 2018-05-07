using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Queue.Logic;
using Xlent.Lever.Libraries2.Core.Queue.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Storage
{
    [TestClass]
    public class MemoryQueueTest : TestIQueue
    {
        private MemoryQueue<TestItemBare> _queue;

        [TestInitialize]
        public void Inititalize()
        {
            _queue = new MemoryQueue<TestItemBare>("test-queue");
        }

        protected override ICompleteQueue<TestItemBare> Queue => _queue;
    }
}
