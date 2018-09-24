using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Context;
using Xlent.Lever.Libraries2.Core.Queue.Logic;

// ReSharper disable ExplicitCallerInfoArgument

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// A convenience class for logging.
    /// </summary>
    public static class Log
    {
        private static readonly TraceSourceLogger TraceSourceLogger = new TraceSourceLogger();
        private static readonly ConsoleLogger ConsoleLogger = new ConsoleLogger();
        private static readonly AsyncLocal<bool> LoggingInProgress = new AsyncLocal<bool> { Value = false };
        private static readonly MemoryQueue<LogBatch> LogQueue = new MemoryQueue<LogBatch>("LogQueue", LogFailSafeAsync);
        private static bool _applicationValidated;
        private static readonly AsyncLocal<LogBatch> LogBatch = new AsyncLocal<LogBatch> { Value = null };

        /// <summary>
        /// This is a property specifically for unit testing.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static bool OnlyForUnitTest_LoggingInProgress
        {
            get
            {
                FulcrumAssert.IsTrue(FulcrumApplication.IsInDevelopment, null,
                    "This property must only be used in unit tests.");
                return LoggingInProgress.Value;
            }
        }

        /// <summary>
        /// This is a property specifically for unit testing.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static bool OnlyForUnitTest_HasBackgroundWorkerForLogging
        {
            get
            {
                FulcrumAssert.IsTrue(FulcrumApplication.IsInDevelopment, null,
                    "This property must only be used in unit tests.");
                return LogQueue.OnlyForUnitTest_HasBackgroundWorkerForLogging;
            }
        }

        /// <summary>
        /// Recommended <see cref="IFulcrumFullLogger"/> for unit testing.
        /// </summary>
        public static IFulcrumFullLogger RecommendedForUnitTest { get; } = ConsoleLogger;

        /// <summary>
        /// Recommended <see cref="IFulcrumFullLogger"/> for developing an application. For testenvironments and production, we recommend the Xlent.Lever.Logger capability.
        /// </summary>
        public static IFulcrumFullLogger RecommendedForNetFramework { get; } = TraceSourceLogger;

        /// <summary>
        /// Verbose logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        public static void LogVerbose(
            string message,
            Exception exception = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogOnLevel(LogSeverityLevel.Verbose, message, exception, memberName, filePath, lineNumber);
        }

        /// <summary>
        /// Information logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        public static void LogInformation(
            string message,
            Exception exception = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogOnLevel(LogSeverityLevel.Information, message, exception, memberName, filePath, lineNumber);
        }

        /// <summary>
        /// Warning logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        public static void LogWarning(
            string message,
            Exception exception = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogOnLevel(LogSeverityLevel.Warning, message, exception, memberName, filePath, lineNumber);
        }

        /// <summary>
        /// Error logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        public static void LogError(
            string message,
            Exception exception = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogOnLevel(LogSeverityLevel.Error, message, exception, memberName, filePath, lineNumber);
        }

        /// <summary>
        /// Critical logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        public static void LogCritical(
            string message,
            Exception exception = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogOnLevel(LogSeverityLevel.Critical, message, exception, memberName, filePath, lineNumber);
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
        public static void LogOnLevel(
            LogSeverityLevel severityLevel,
            string message,
            Exception exception = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_applicationValidated)
            {
                FulcrumApplication.Validate();
                _applicationValidated = true;
            }
            var log = CreateLogInstanceInformation(severityLevel, message, exception, memberName, filePath, lineNumber);
            if (LoggingInProgress.Value)
            {
                if (log.IsGreateThanOrEqualTo(FulcrumApplication.Setup.LogSeverityLevelThreshold))
                {
                    var logBatch = new LogBatch(log);
                    const string abortMessage =
                        "Log recursion! Will not send the following inner log to the configured logger.";
                    FallbackToSimpleLoggingFailSafe(abortMessage, logBatch);
                }
            }
            else
            {
                LogToBatchOrImmediately(log);
            }
        }

        private static void LogToBatchOrImmediately(LogRecord log)
        {
            var logBatch = new LogBatch(log);
            if (LogBatch.Value != null)
            {
                if (LogBatch.Value.Context.Equals(logBatch.Context))
                {
                    LogBatch.Value.Records.Add(log);
                    return;
                }
                ForceExecuteBatch(
                    "A log was added to the batch that didn't have the same context as the other logs in the batch.",
                    $"All the following logs (up to the next {nameof(StartBatch)}) will be logged individually, i.e. not in a batch.");
                FulcrumAssert.IsNull(LogBatch.Value);
            }

            if (!log.IsGreateThanOrEqualTo(FulcrumApplication.Setup.LogSeverityLevelThreshold)) return;
            LogQueue.AddMessage(logBatch);
        }

        /// <summary>
        /// Start a new batch of logs. All the following logs will be saved internally and will not be activated until you call <see cref="ExecuteBatch"/>.
        /// </summary>
        /// <remarks>
        /// There are some cases where ExecuteBatch() will be called automatically:
        /// - If StartBatch() is called again
        /// - If the context for a new log compared with the current batch.
        /// </remarks>
        public static void StartBatch()
        {
            if (LogBatch.Value != null)
            {
                ForceExecuteBatch(
                    $"Calling {nameof(StartBatch)} a second time, without a call to {nameof(ExecuteBatch)} in between.",
                    "None. We have activated the current batch and started a new one.");
            }
            FulcrumAssert.IsNull(LogBatch.Value);
            LogBatch.Value = new LogBatch();
        }

        /// <summary>
        /// Activate the logs that have been saved internally since the latest <see cref="StartBatch"/>.
        /// </summary>
        public static void ExecuteBatch()
        {
            if (LogBatch.Value == null) return;
            LogBatch.Value.FilterByThreshold();
            LogQueue.AddMessage(LogBatch.Value);
            LogBatch.Value = null;
        }

        private static void ForceExecuteBatch(string reason, string consequence)
        {
            ExecuteBatch();
            Log.LogWarning("Logging was internally forced to execute a batch of logs.Reason:\r" +
                           $"{reason}\r" +
                           "Probably worth investigating, because it was not expected to happen. Consequence:\r" +
                           consequence);
        }

        internal static LogRecord CreateLogInstanceInformation(
            LogSeverityLevel severityLevel,
            string message,
            Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var correlationValueProvider = new CorrelationIdValueProvider();
            var logInstanceInformation = new LogRecord
            {
                CorrelationId = correlationValueProvider.CorrelationId,
                TimeStamp = DateTimeOffset.Now,
                SeverityLevel = severityLevel,
                Message = message,
                Location = $"{memberName} in {filePath} line {lineNumber}",
                Exception = exception
            };

            if (logInstanceInformation.IsGreateThanOrEqualTo(LogSeverityLevel.Error))
            {
                logInstanceInformation.StackTrace = Environment.StackTrace;
            }

            return logInstanceInformation;
        }

        /// <summary>
        /// Safe logging of messages. Will check for errors, but never throw an exception. If the log can't be made with the chosen logger, a fallback log will be created.
        /// </summary>
        /// <param name="logBatch">Information about the logging.</param>
        private static async Task LogFailSafeAsync(LogBatch logBatch)
        {
            if (logBatch?.Records == null || logBatch.Records.Count == 0) return;
            try
            {
                //ReSharper disable once ObjectCreationAsStatement
                new TenantConfigurationValueProvider
                {
                    Tenant = logBatch.Context.ClientTenant ?? logBatch.Context.ApplicationTenant,
                    CallingClientName = logBatch.Context.ClientName
                };
                // ReSharper disable once ObjectCreationAsStatement
                new CorrelationIdValueProvider
                {
                    CorrelationId = logBatch.Context.CorrelationId
                };
                await LogWithConfiguredLoggerFailSafeAsync(logBatch);
                foreach (var log in logBatch.Records)
                {
                    var formattedMessage = FormatMessageFailSafe(logBatch, log);
                    AlsoLogWithTraceSourceInDevelopment(log.SeverityLevel, formattedMessage);
                }
            }
            catch (Exception e)
            {
                FallbackToSimpleLoggingFailSafe($"{nameof(LogFailSafeAsync)} caught an exception.", logBatch, e);
            }
        }

        private static async Task LogWithConfiguredLoggerFailSafeAsync(LogBatch logBatch)
        {
            try
            {
                try
                {
                    FulcrumAssert.AreEqual(false, LoggingInProgress.Value);
                }
                catch (Exception e)
                {
                    FallbackToSimpleLoggingFailSafe(LogSeverityLevel.Error, e.Message);
                }
                
                LoggingInProgress.Value = true;
                await FulcrumApplication.Setup.FullLogger.LogAsync(logBatch);
            }
            catch (Exception e)
            {
                FallbackToSimpleLoggingFailSafe(
                    $"{nameof(LogWithConfiguredLoggerFailSafeAsync)} caught an exception from logger {FulcrumApplication.Setup.FullLogger?.GetType().FullName}.",
                    logBatch, e);
            }
            finally
            {
                LoggingInProgress.Value = false;
            }
        }

        private static void AlsoLogWithTraceSourceInDevelopment(LogSeverityLevel severityLevel, string formattedMessage)
        {
            if (FulcrumApplication.Setup.FullLogger?.GetType() == typeof(TraceSourceLogger)) return;
            if (!FulcrumApplication.IsInDevelopment) return;
            TraceSourceLogger.Log(severityLevel, formattedMessage);
        }

        /// <summary>
        /// Create a formatted message based on <paramref name="log"/>.
        /// </summary>
        /// <param name="logBatch">Information about the context for the log</param>
        /// <param name="log">Information about the log.</param>
        /// <returns>A formatted message, never null or empty</returns>
        public static string FormatMessageFailSafe(LogBatch logBatch, LogRecord log)
        {
            if (log == null) return null;
            try
            {
                return log.ToLogString(false, logBatch.Context);
            }
            catch (Exception e)
            {
                return $"Formatting message failed ({e.Message}): {log.Message}";
            }
        }


        /// <summary>
        /// Use this method to log when the original logging method fails.
        /// </summary>
        /// <param name="message">What went wrong with logging</param>
        /// <param name="logBatch">The message to log.</param>
        /// <param name="exception">If what went wrong had an exception</param>
        private static void FallbackToSimpleLoggingFailSafe(string message, LogBatch logBatch, Exception exception = null)
        {
            if (logBatch?.Records == null || logBatch?.Records.Count == 0) return;
            try
            {
                var failSafeId = Guid.NewGuid().ToString();

                var logWithHighestSeverity = logBatch.GetLogWithHighestSeverityLevel();
                var totalMessage = $"Begin: {failSafeId}. Timestamp: {DateTimeOffset.Now}\r";
                totalMessage += string.IsNullOrEmpty(logBatch.Context.CorrelationId) ? "" : $"CorrelationId: {logBatch.Context.CorrelationId}\r";
                totalMessage += message == null ? "" : $"{message}\r";
                if (exception != null)
                {
                    totalMessage += $"Unexpected exception when logging:\r{exception.ToLogString()}";
                }
                else
                {
                    totalMessage += "The logging mechanism itself failed and is using a fallback method.";
                }

                if (exception != null || logWithHighestSeverity.IsGreateThanOrEqualTo(LogSeverityLevel.Error))
                {
                    totalMessage += $"\rStack trace up to this point:\r{Environment.StackTrace}";
                }

                // If a message of warning or higher ends up here means it is critical, since this log will not end up in the normal log.
                var severityLevel = logWithHighestSeverity.IsGreateThanOrEqualTo(LogSeverityLevel.Warning) ? LogSeverityLevel.Critical : LogSeverityLevel.Warning;
                FallbackToSimpleLoggingFailSafe(severityLevel, totalMessage);
                foreach (var log in logBatch.Records)
                {
                    FallbackToSimpleLoggingFailSafe(log.SeverityLevel, log.Message);
                }
                FallbackToSimpleLoggingFailSafe(severityLevel, $"End: {failSafeId}");
            }
            catch (Exception)
            {
                // We give up
            }
        }


        /// <summary>
        /// Use this method to log when the original logging method fails.
        /// </summary>
        private static void FallbackToSimpleLoggingFailSafe(LogSeverityLevel severityLevel, string message)
        {

            try
            {
                try
                {
                    if (FulcrumApplication.IsInDevelopment && FulcrumApplication.Setup.FullLogger != RecommendedForNetFramework) RecommendedForUnitTest.Log(severityLevel, message);
                    else RecommendedForNetFramework.Log(severityLevel, message);
                }
                catch (Exception e)
                {
                    var totalMessage = $"{message}\r{e.ToLogString()}";
                    Console.WriteLine($"{totalMessage}");
                }
            }
            catch (Exception)
            {
                // We give up
            }
        }
    }
}

