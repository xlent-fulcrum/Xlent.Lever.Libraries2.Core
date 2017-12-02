using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Queue.Logic;
using Xlent.Lever.Libraries2.Core.Queue.Model;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Storage
{
    [TestClass]
    public class MemoryQueueTest : TestIQueue<PersonStorableItem<Guid>>
    {
        private MemoryQueue<PersonStorableItem<Guid>> _queue;

        [TestInitialize]
        public void Inititalize()
        {
            _queue = new MemoryQueue<PersonStorableItem<Guid>>("test-queue");
        }

        protected override ICompleteQueue<PersonStorableItem<Guid>> Queue => _queue;
    }
}
