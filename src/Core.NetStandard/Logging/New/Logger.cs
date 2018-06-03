using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Misc;
using Xlent.Lever.Libraries2.Core.Queue.Logic;
using Xlent.Lever.Libraries2.Core.Queue.Model;
// ReSharper disable ExplicitCallerInfoArgument

namespace Xlent.Lever.Libraries2.Core.Logging.New
{
    /// <summary>
    /// A convenience class for logging.
    /// </summary>
    public class Logger : ILogger
    {
        private readonly MemoryQueue<LogBatch> _logQueue;
        private readonly AsyncLocal<LogBatch> _logBatch = new AsyncLocal<LogBatch> { Value = null };
        private bool _applicationValidated;

        /// <summary>
        /// This is a property specifically for unit testing.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool OnlyForUnitTest_HasBackgroundWorkerForLogging
        {
            get
            {
                FulcrumAssert.IsTrue(FulcrumApplication.IsInDevelopment, null,
                    "This property must only be used in unit tests.");
                return _logQueue.OnlyForUnitTest_HasBackgroundWorkerForLogging;
            }
        }

        /// <summary>
        /// A logger that puts logs on a memory queue and sends them to the appropriate service as a background job.
        /// </summary>
        public Logger()
        {
            _logQueue = new MemoryQueue<LogBatch>("LogQueue", LogQueueReader.LogFailSafeAsync);
        }

        /// <summary>
        /// Verbose logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        public void LogVerbose(
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            LogOnLevel(LogSeverityLevel.Verbose, message, exception, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Information logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        public void LogInformation(
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            LogOnLevel(LogSeverityLevel.Information, message, exception, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Warning logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        public void LogWarning(
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            LogOnLevel(LogSeverityLevel.Warning, message, exception, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Error logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        public void LogError(
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            LogOnLevel(LogSeverityLevel.Error, message, exception, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Critical logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        public void LogCritical(
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            LogOnLevel(LogSeverityLevel.Critical, message, exception, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Safe logging of a message. Will check for errors, but never throw an exception. If the log can't be made with the chosen logger, a fallback log will be created.
        /// </summary>
        /// <param name="severityLevel">The severity level for this log.</param>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        [StackTraceHidden]
        public void LogOnLevel(
            LogSeverityLevel severityLevel,
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (!_applicationValidated)
            {
                FulcrumApplication.Validate();
                _applicationValidated = true;
            }

            var logRecord = new LogRecord(severityLevel, message, exception, lineNumber, filePath, memberName);
            AddToBatchOrLogImmediately(logRecord);
        }

        /// <summary>
        /// Start a new batch of logs. All the following logs will be saved internally and will not be activated until you call <see cref="ExecuteBatch"/>.
        /// </summary>
        /// <remarks>
        /// There are some cases where ExecuteBatch() will be called automatically:
        /// - If StartBatch() is called again
        /// - If the context for a new log compared with the current batch.
        /// </remarks>
        public void StartBatch()
        {
            if (_logBatch.Value != null)
            {
                ForceExecuteBatch(
                    $"Calling {nameof(StartBatch)} a second time, without a call to {nameof(ExecuteBatch)} in between.",
                    "None. We have activated the current batch and started a new one.");
            }
            FulcrumAssert.IsNull(_logBatch.Value);
            _logBatch.Value = new LogBatch();
        }

        /// <summary>
        /// Activate the logs that have been saved internally since the latest <see cref="StartBatch"/>.
        /// </summary>
        public void ExecuteBatch()
        {
            if (_logBatch.Value == null) return;
            _logBatch.Value.FilterByThreshold();
            _logQueue.AddMessage(_logBatch.Value);
            _logBatch.Value = null;
        }

        private void AddToBatchOrLogImmediately(LogRecord log)
        {
            var logBatch = new LogBatch(log);
            if (LogQueueReader.IsRecursive)
            {
                const string abortMessage = "Log recursion! Detected a log within a log. The inner log could not be processed as intended, so it is logged locally. ";
                LogQueueReader.FallbackToSimpleLoggingFailSafe(abortMessage, logBatch);
                return;
            }
            if (_logBatch.Value != null)
            {
                if (_logBatch.Value.Context.Equals(logBatch.Context))
                {
                    _logBatch.Value.Records.Add(log);
                    return;
                }
                ForceExecuteBatch(
                    "A log was added to the batch that didn't have the same context as the other logs in the batch.",
                    $"All the following logs (up to the next {nameof(StartBatch)}) will be logged individually, i.e. not in a batch.");
                FulcrumAssert.IsNull(_logBatch.Value);
            }

            if (!log.IsGreateThanOrEqualTo(FulcrumApplication.Setup.LogSeverityLevelThreshold)) return;
            _logQueue.AddMessage(logBatch);
        }

        private void ForceExecuteBatch(string reason, string consequence)
        {
            ExecuteBatch();
            Log.LogWarning("Logging was internally forced to execute a batch of logs.Reason:\r" +
                           $"{reason}\r" +
                           "Probably worth investigating, because it was not expected to happen. Consequence:\r" +
                           consequence);
        }
    }
}
