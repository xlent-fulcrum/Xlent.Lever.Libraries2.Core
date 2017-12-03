using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Queue.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet
{
    [TestClass]
    public abstract class TestIQueue
    {
        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICompleteQueue<TestItemBare> Queue { get; }

        [TestMethod]
        public async Task GetDoesNotBlock()
        {
            var getTask = Queue.GetOneMessageNoBlockAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(getTask.IsCompleted, "Expected the method to finish quickly.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(await getTask);
        }

        [TestMethod]
        public async Task PeekDoesNotBlock()
        {
            var getTask = Queue.PeekNoBlockAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(getTask.IsCompleted, "Expected the method to finish quickly.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(await getTask);
        }

        [TestMethod]
        public async Task MessageGetsThrough()
        {
            var message = new TestItemBare();
            message.InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            await Queue.AddMessageAsync(message);
            var result = await Queue.GetOneMessageNoBlockAsync();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(message, result);
        }

        [TestMethod]
        public async Task PeekAndGet()
        {
            var message = new TestItemBare();
            message.InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            await Queue.AddMessageAsync(message);
            var result = await Queue.PeekNoBlockAsync();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(message, result);
            result = await Queue.GetOneMessageNoBlockAsync();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(message, result);
        }

        [TestMethod]
        public async Task ClearQueue()
        {
            var message = new TestItemBare();
            message.InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            await Queue.AddMessageAsync(message);
            await Queue.ClearAsync();
            var getTask = Queue.GetOneMessageNoBlockAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(getTask.IsCompleted, "Expected the method to finish quickly.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(await getTask);
        }
    }
}
