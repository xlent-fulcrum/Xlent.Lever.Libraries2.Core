using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Logging;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Logging
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
            Log.LogInformation("Top level logging 1, will be followed by recursive logging that should be detected");
            while (RecursiveLogger.IsRunning) Thread.Sleep(TimeSpan.FromMilliseconds(100));
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            UT.Assert.IsFalse(Log.OnlyForUnitTest_LoggingInProgress, "Should never happen between logs");
            Log.LogInformation("Top level logging 2, will be followed by recursive logging that should be detected");
            UT.Assert.IsFalse(Log.OnlyForUnitTest_LoggingInProgress, "Should never happen between logs");
            while (RecursiveLogger.IsRunning) Thread.Sleep(TimeSpan.FromMilliseconds(100));
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            UT.Assert.IsFalse(RecursiveLogger.HasFailed, RecursiveLogger.Message);
        }

        [TestMethod]
        public void ParallelLogging()
        {
            FulcrumApplication.Setup.FullLogger = new RecursiveLogger();
            RecursiveLogger.IsRunning = true;
            Log.LogInformation("Top level logging 1");
            UT.Assert.IsFalse(Log.OnlyForUnitTest_LoggingInProgress, "Should never happen between logs");
            Log.LogInformation("Top level logging 2");
            UT.Assert.IsFalse(Log.OnlyForUnitTest_LoggingInProgress, "Should never happen between logs");
            while (RecursiveLogger.IsRunning) Thread.Sleep(TimeSpan.FromMilliseconds(100));
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            UT.Assert.IsFalse(RecursiveLogger.HasFailed, RecursiveLogger.Message);
        }

        // This test was supposed to verify that the logs run in parallel. We have removed that capability as long as the memory queue can't be run in parallel.
        [Ignore]
        [TestMethod]
        public void VerifyThatSlowLogsRunInParallelAreFast()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            LogSpecifiedNumberOfLogs(1000);
            while (Log.OnlyForUnitTest_HasBackgroundWorkerForLogging) Thread.Sleep(TimeSpan.FromMilliseconds(100));
            stopWatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"Total time: {stopWatch.Elapsed.TotalSeconds} seconds");
        }

        [TestMethod]
        public void Batch()
        {

            var mockLogger = new Mock<IFulcrumFullLogger>();
            mockLogger.Setup(logger =>
                logger.LogAsync(It.Is<LogBatch>(logBatch => logBatch.Records != null && logBatch.Records.Count == 5))).Returns(Task.CompletedTask);
            FulcrumApplication.Setup.FullLogger = mockLogger.Object;
            Log.StartBatch();
            Log.LogVerbose("Verbose");
            Log.LogInformation("Information");
            Log.LogWarning("Warning");
            Log.LogError("Error");
            Log.LogCritical("Critical");
            Log.ExecuteBatch();
            while (Log.OnlyForUnitTest_HasBackgroundWorkerForLogging) Thread.Sleep(TimeSpan.FromMilliseconds(100));
            mockLogger.Verify();
        }

        private static void LogSpecifiedNumberOfLogs(int numberOfLogs)
        {
            FulcrumApplication.Setup.FullLogger = new SlowLogger(TimeSpan.FromSeconds(1.0));
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < numberOfLogs; i++)
            {
                Log.LogInformation($"Log number {i + 1}");
            }
            stopWatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"Time for {numberOfLogs} log messages: {stopWatch.Elapsed.TotalSeconds} seconds");
        }
    }

    internal class SlowLogger : IFulcrumFullLogger
    {
        private readonly TimeSpan _delay;

        public SlowLogger(TimeSpan delay)
        {
            _delay = delay;
        }

        public void Log(LogSeverityLevel logSeverityLevel, string message)
        {
            throw new NotImplementedException();
        }

        public async Task LogAsync(LogBatch logBatch)
        {
            if (logBatch?.Records == null) return;
            foreach (var log in logBatch.Records)
            {
                await Task.Delay(_delay);
                Console.Write($"{log.Message} ");
            }
        }
    }

    internal class RecursiveLogger : IFulcrumFullLogger
    {
        public static string Message { get; private set; }
        public static bool HasFailed { get; private set; }
        public static bool IsRunning { get; set; }
        public static int InstanceCount { get; set; }
        private static readonly object ClassLock = new object();

        /// <inheritdoc />
        public void Log(LogSeverityLevel logSeverityLevel, string message)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task LogAsync(LogBatch logBatch)
        {
            if (logBatch?.Records == null) return Task.CompletedTask;
            foreach (var log in logBatch.Records)
            {

                lock (ClassLock)
                {
                    InstanceCount++;
                    IsRunning = true;
                }

                var recursiveLogMessage = "Recursive log message";
                var recursive = log.Message == recursiveLogMessage;
                if (recursive)
                {
                    if (!HasFailed)
                    {
                        HasFailed = true;
                        Message =
                            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                            $"The {nameof(LogAsync)}() method should never be called recursively. {nameof(recursive)} = {recursive}, {nameof(Libraries2.Core.Logging.Log.OnlyForUnitTest_LoggingInProgress)} = {Libraries2.Core.Logging.Log.OnlyForUnitTest_LoggingInProgress}";
                    }
                }

                Console.WriteLine(log.Message);
                // Try to provoke a recursive log call of this method
                if (!HasFailed) Libraries2.Core.Logging.Log.LogError(recursiveLogMessage);
                lock (ClassLock)
                {
                    InstanceCount--;
                    if (InstanceCount < 0)
                    {
                        if (!HasFailed)
                        {
                            HasFailed = true;
                            Message =
                                $"Unexpectedly had an {nameof(InstanceCount)} with value {InstanceCount} < 0";
                        }

                        InstanceCount = 0;
                    }

                    if (InstanceCount == 0) IsRunning = false;
                }
            }

            return Task.CompletedTask;
        }
    }
}
