using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Context;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    [TestClass]
    public class TestLogging
    {
        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(TestLogging).FullName);
        }

        [TestMethod]
        public void DetectRecursiveLogging()
        {
            FulcrumApplication.Setup.FullLogger = new RecursiveLogger();
            RecursiveLogger.IsRunning = true;
            Logging.Log.LogInformation("Top level logging 1, will be followed by recursive logging that should be detected");
            while (RecursiveLogger.IsRunning) Thread.Sleep(TimeSpan.FromMilliseconds(100));
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            UT.Assert.IsFalse(Logging.Log.OnlyForUnitTest_LoggingInProgress, "Should never happen between logs");
            Logging.Log.LogInformation("Top level logging 2, will be followed by recursive logging that should be detected");
            UT.Assert.IsFalse(RecursiveLogger.HasFailed, RecursiveLogger.Message);
        }
    }
}
