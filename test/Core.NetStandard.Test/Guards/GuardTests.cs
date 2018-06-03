using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Guards;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.Logging.New;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
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

        [TestMethod]
        public void TestIsNotNullOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsNotNull("");
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNullNotFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsNotNull(null);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsDefaultValueOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsDefaultValue(0);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsDefaultValueFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsDefaultValue(1);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotDefaultValueOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsNotDefaultValue(1);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotDefaultValueFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsNotDefaultValue(0);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNullOrWhiteSpaceWithNullOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsNullOrWhiteSpace(null);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNullOrWhiteSpaceWithEmptyOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsNullOrWhiteSpace("");
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNullOrWhiteSpaceWithShiteSpaceOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsNullOrWhiteSpace(" \t\r\n\v");
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNullOrWhiteSpaceFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsNullOrWhiteSpace("Fail");
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotNullOrWhiteSpaceOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsNotNullOrWhiteSpace("Ok");
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotNullOrWhiteSpaceWithNullFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsNotNullOrWhiteSpace(null);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotNullOrWhiteSpaceWithEmptyFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsNotNullOrWhiteSpace("");
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotNullOrWhiteSpaceWithShiteSpaceFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsNotNullOrWhiteSpace(" \t\r\n\v");
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestAreEqualOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            var a = new Tenant("org", "env");
            var b = new Tenant("org", "env");
            guard.AreEqual(a, b);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestAreEqualFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            var a = new Tenant("org", "env-1");
            var b = new Tenant("org", "env-2");
            guard.AreEqual(a, b);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestAreNotEqualOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            var a = new Tenant("org", "env-1");
            var b = new Tenant("org", "env-2");
            guard.AreNotEqual(a, b);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestAreNotEqualFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            var a = new Tenant("org", "env");
            var b = new Tenant("org", "env");
            guard.AreNotEqual(a, b);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsAssignableToOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsAssignableTo(typeof(ArgumentException), typeof(Exception));
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsAssignableToFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsAssignableTo(typeof(Exception), typeof(ArgumentException));
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotAssignableToOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsNotAssignableTo(typeof(Exception), typeof(ArgumentException));
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotAssignableToFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsNotAssignableTo(typeof(ArgumentException), typeof(Exception));
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

        [TestMethod]
        public void TestIsInstanceOfOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsInstanceOf(new ArgumentException(), typeof(Exception));
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsInstanceOfFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsInstanceOf(new Exception(), typeof(ArgumentException));
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotInstanceOfOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsNotInstanceOf(new Exception(), typeof(ArgumentException));
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotInstanceOfFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsNotInstanceOf(new ArgumentException(), typeof(Exception));
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanIntOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsLessThan(1, 2);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanIntFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsLessThan(1, 1);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanDoubleOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsLessThan(1.0, 1.1);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanDoubleFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsLessThan(1.0, 1.0);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanDateTimeOffsetOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsLessThan(DateTimeOffset.Now, DateTimeOffset.MaxValue);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanDateTimeOffsetFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsLessThan(DateTimeOffset.MaxValue, DateTimeOffset.MaxValue);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanOrEqualToIntOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsLessThanOrEqualTo(1, 1);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanOrEqualToIntFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsLessThanOrEqualTo(1, 0);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanOrEqualToDoubleOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsLessThanOrEqualTo(1.0, 1.0);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanOrEqualToDoubleFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsLessThanOrEqualTo(1.0, 0.9);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanOrEqualToDateTimeOffsetOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsLessThanOrEqualTo(DateTimeOffset.MaxValue, DateTimeOffset.MaxValue);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsLessThanOrEqualToDateTimeOffsetFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsLessThanOrEqualTo(DateTimeOffset.MaxValue, DateTimeOffset.Now);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanIntOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsGreaterThan(2, 1);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanIntFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsGreaterThan(1, 1);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanDoubleOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsGreaterThan(1.1, 1.0);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanDoubleFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsGreaterThan(1.0, 1.0);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanDateTimeOffsetOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsGreaterThan(DateTimeOffset.Now, DateTimeOffset.MinValue);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanDateTimeOffsetFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsGreaterThan(DateTimeOffset.MinValue, DateTimeOffset.MinValue);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanOrEqualToIntOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsGreaterThanOrEqualTo(1, 1);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanOrEqualToIntFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsGreaterThanOrEqualTo(0, 1);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanOrEqualToDoubleOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsGreaterThanOrEqualTo(1.0, 1.0);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanOrEqualToDoubleFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsGreaterThanOrEqualTo(0.9, 1.0);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanOrEqualToDateTimeOffsetOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsGreaterThanOrEqualTo(DateTimeOffset.MinValue, DateTimeOffset.MinValue);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsGreaterThanOrEqualToDateTimeOffsetFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsGreaterThanOrEqualTo(DateTimeOffset.MinValue, DateTimeOffset.Now);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsRegexMatchOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsRegexMatch("IsMatch", "M");
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsRegexMatchFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsRegexMatch("IsNotMatch", "m");
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotRegexMatchOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            guard.IsNotRegexMatch("IsNotMatch", "m");
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotRegexMatchFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            guard.IsNotRegexMatch("IsMatch", "M");
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsValidOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            var validatable = new ImplementsValidatable { Mandatory = "Ok" };
            guard.IsValid(validatable);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsValidFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            var validatable = new ImplementsValidatable { Mandatory = null };
            guard.IsValid(validatable);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsValidEnumerationOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            var validatable = new ImplementsValidatable { Mandatory = "Ok" };
            guard.IsValid(new object[]{validatable});
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsValidEnumerationFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            var validatable = new ImplementsValidatable { Mandatory = null };
            guard.IsValid(new object[] { validatable });
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotValidOk()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectNoLog(loggerMock);
            var validatable = new ImplementsValidatable { Mandatory = null };
            guard.IsNotValid(validatable);
            loggerMock.Verify();
        }

        [TestMethod]
        public void TestIsNotValidFail()
        {
            var loggerMock = new Mock<ILogger>();
            var guard = new Guard(LogSeverityLevel.Warning);
            SetupExpectWarningLog(loggerMock);
            var validatable = new ImplementsValidatable { Mandatory = "Ok" };
            guard.IsNotValid(validatable);
            loggerMock.Verify();
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
