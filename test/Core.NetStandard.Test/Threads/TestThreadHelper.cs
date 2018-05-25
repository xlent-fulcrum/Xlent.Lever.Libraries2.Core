using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Threads;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Threads
{
    [TestClass]
    public class TestThreadHelper
    {
        private ICorrelationIdValueProvider _provider;

        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestThreadHelper).FullName);
            FulcrumApplication.Setup.ContextValueProvider = new AsyncLocalValueProvider();
            FulcrumApplication.Setup.ThreadHandler = new BasicThreadHandler();
            _provider = new CorrelationIdValueProvider
            {
                CorrelationId = Guid.NewGuid().ToString()
            };
        }

        [TestMethod]
        public void ThreadsCanAccessContext()
        {
            var correlationId = Guid.NewGuid().ToString();
            _provider.CorrelationId = correlationId;
            var done = false;
            var canAccess = false;
            FulcrumApplication.Setup.ThreadHandler.FireAndForget(token =>
            {
                var provider = new CorrelationIdValueProvider();
                canAccess = correlationId == provider.CorrelationId;
                done = true;
            });
            while (!done)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }
            UT.Assert.IsTrue(canAccess);
        }

        [TestMethod]
        public void TestMaxDepth()
        {
            var correlationId = Guid.NewGuid().ToString();
            _provider.CorrelationId = correlationId;

            const int tries = 15;
            var count = 0;
            var actionCalled = new ManualResetEvent(false);
            for (var i = 0; i <= tries; i++)
            {
                ThreadHelper.FireAndForgetWithExpensiveContextPreservation(() =>
                {
                    lock (correlationId)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(100));
                        var provider = new CorrelationIdValueProvider();
                        var canAccess = correlationId == provider.CorrelationId;
                        if (++count == tries) actionCalled.Set();
                        UT.Assert.IsTrue(canAccess);
                        Console.WriteLine($"count: {count}");
                    }
                });
                Console.WriteLine($"i: {i}");
            }
            UT.Assert.IsTrue(actionCalled.WaitOne(TimeSpan.FromSeconds(3)), $"Could not finish the {tries} tasks");

        }
    }
}
