using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Context;
using Xlent.Lever.Libraries2.Core.Queue.Logic;
using Xlent.Lever.Libraries2.Core.Queue.Model;

// ReSharper disable ExplicitCallerInfoArgument

namespace Xlent.Lever.Libraries2.Core.Logging.New
{
    /// <summary>
    /// A convenience class for logging.
    /// </summary>
    public static class LogQueueReader
    {
        private static readonly TraceSourceLogger TraceSourceLogger = new TraceSourceLogger();
        private static readonly ConsoleLogger ConsoleLogger = new ConsoleLogger();
        private static readonly AsyncLocal<bool> LoggingInProgress = new AsyncLocal<bool> { Value = false };

        /// <summary>
        /// Recommended <see cref="IFulcrumFullLogger"/> for unit testing.
        /// </summary>
        public static IFulcrumFullLogger RecommendedForUnitTest { get; } = ConsoleLogger;

        /// <summary>
        /// Recommended <see cref="IFulcrumFullLogger"/> for developing an application. For testenvironments and production, we recommend the Xlent.Lever.Logger capability.
        /// </summary>
        public static IFulcrumFullLogger RecommendedForNetFramework { get; } = TraceSourceLogger;

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
        internal static bool IsRecursive => LoggingInProgress.Value;

        /// <summary>
        /// Safe logging of messages. Will check for errors, but never throw an exception. If the log can't be made with the chosen logger, a fallback log will be created.
        /// </summary>
        /// <param name="logBatch">Information about the logging.</param>
        public static async Task LogFailSafeAsync(LogBatch logBatch)
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
            TraceSourceLogger.Log(severityLevel, formattedMessage);
        }

        /// <summary>
        /// Create a formatted message based on <paramref name="log"/>.
        /// </summary>
        /// <param name="logBatch">Information about the context for the log</param>
        /// <param name="log">Information about the log.</param>
        /// <returns>A formatted message, never null or empty</returns>
        private static string FormatMessageFailSafe(LogBatch logBatch, LogRecord log)
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
        internal static void FallbackToSimpleLoggingFailSafe(string message, LogBatch logBatch, Exception exception = null)
        {
            if (logBatch?.Records == null || logBatch?.Records.Count == 0) return;
            try
            {
                var logWithHighestSeverity = logBatch.GetLogWithHighestSeverityLevel();
                var totalMessage = message == null ? "" : $"{message}\r";
                if (exception != null)
                {
                    totalMessage += $"\r\rUnexpected excpeption when logging:\r{exception.ToLogString()}";
                }
                else
                {
                    totalMessage += "\r\rThe logging mechanism itself failed and is using a fallback method for one or more logs.";
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

