using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Guards;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.Logging.New;
// ReSharper disable ExplicitCallerInfoArgument

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Guards
{
    [TestClass]
    public class GuardTests
    {
        private Mock<ILogger> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(nameof(GuardTests));
            _loggerMock = new Mock<ILogger>();
            NexusLink.Nexus.Logger = _loggerMock.Object;
        }
        [TestMethod]
        public void WarningOnly()
        {
            _loggerMock.Setup(logger => logger.LogOnLevel(
                It.IsAny<LogSeverityLevel>(),//(It.Is<LogSeverityLevel>(level => level == LogSeverityLevel.Warning),
                It.IsAny<string>(),
                It.IsAny<Exception>(), //It.Is<Exception>(e => e == null),
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));
            var guard = new Guard(LogSeverityLevel.Warning);
            guard.Fail();
            _loggerMock.VerifyAll();
        }
    }
}
