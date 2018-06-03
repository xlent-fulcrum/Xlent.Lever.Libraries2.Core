using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Guards;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.Logging.New;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable ExplicitCallerInfoArgument

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Guards
{
    [TestClass]
    public class GuardTests
    {
        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(nameof(GuardTests));
        }

        [TestMethod]
        public void LogWarningOnly()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.Fail();
            loggerMock.Verify();
        }

        [TestMethod]
        public void LogWarningAndThrowException()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning, typeof(TestException));
            SetupExpectWarningLog(loggerMock);
            try
            {
                guard.Fail();
                UT.Assert.Fail(
                    $"Expected that an exception should be thrown.");
            }
            catch (TestException)
            {
                // As exepected
            }
            catch (Exception e)
            {
                UT.Assert.Fail(
                    $"Expected exception {typeof(TestException).Name}, but got {e.GetType().Name}.");
            }

            loggerMock.Verify();
        }

        [TestMethod]
        public void FulcrumExceptionHasCorrectErrorLocation()
        {
            var guard = new Guard(LogSeverityLevel.Warning, typeof(FulcrumServiceContractException));
            try
            {
                guard.Fail();
                UT.Assert.Fail(
                    $"Expected that an exception should be thrown.");
            }
            catch (FulcrumServiceContractException e)
            {
                UT.Assert.IsTrue(e.ErrorLocation.Contains(nameof(GuardTests)));
                UT.Assert.IsTrue(e.ErrorLocation.Contains(".cs"));
                UT.Assert.IsTrue(e.ErrorLocation.Contains(nameof(FulcrumExceptionHasCorrectErrorLocation)));
                // Show the error location in the test log
                Console.WriteLine(e.ErrorLocation);
            }
            catch (Exception e)
            {
                UT.Assert.Fail(
                    $"Expected exception {typeof(TestException).Name}, but got {e.GetType().Name}.");
            }
        }

        [TestMethod]
        public void TestIsFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.Fail();
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsTrueOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsTrue(true);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsTrueFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsTrue(false);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsFalseOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsFalse(false);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsFalseFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsFalse(true);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNullOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsNull(null);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNullFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsNull("");
            loggerMock.Verify();
        }

        private static void SetupExpectWarningLog(Mock<ILogger> loggerMock)
        {
            NexusLink.Nexus.Logger = loggerMock.Object;
            loggerMock.Setup(logger => logger.LogOnLevel(
                    It.Is<LogSeverityLevel>(level => level == LogSeverityLevel.Warning),
                    It.IsAny<string>(),
                    It.Is<Exception>(e => e == null),
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();
        }

        private static void SetupExpectNoLog(Mock<ILogger> loggerMock)
        {
            NexusLink.Nexus.Logger = loggerMock.Object;
            loggerMock.Setup(logger => logger.LogOnLevel(
                    It.Is<LogSeverityLevel>(level => level == LogSeverityLevel.Warning),
                    It.IsAny<string>(),
                    It.Is<Exception>(e => e == null),
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new AssertFailedException("Did not expect any logging to take place."));
        }
    }
}
