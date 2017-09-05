using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Context;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.Threads
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
            FulcrumApplication.Setup.ThreadHandler.FireAndForget(cancellationToken =>
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
    }
}
