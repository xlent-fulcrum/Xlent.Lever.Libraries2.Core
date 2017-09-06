using System;
using System.Diagnostics;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Context;
using Xlent.Lever.Libraries2.Core.Threads;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// A convenience class for logging.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// The chosen <see cref="IValueProvider"/> to use.
        /// </summary>
        /// <remarks>There are overrides for this, see e.g. in Xlent.Lever.Libraries2.WebApi.ContextValueProvider.</remarks>
        [Obsolete("Use FulcrumApplication.Setup.Logger", true)]
#pragma warning disable 169
        private static IFulcrumLogger _chosenLogger;
#pragma warning restore 169
        private static readonly TraceSourceLogger TraceSourceLogger = new TraceSourceLogger();
        [ThreadStatic]
        private static bool _loggingInProgress;
        private static readonly object ClassLock = new object();

        /// <summary>
        /// Recommended <see cref="IFulcrumFullLogger"/> for developing an application. For testenvironments and production, we recommend the Xlent.Lever.Logger capability.
        /// </summary>
        public static IFulcrumFullLogger RecommendedForNetFramework { get; } = TraceSourceLogger;

        /// <summary>
        /// Verbose logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        public static void LogVerbose(string message, Exception exception = null)
        {
            LogOnLevel(LogSeverityLevel.Verbose, message, exception);
        }

        /// <summary>
        /// Information logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        public static void LogInformation(string message, Exception exception = null)
        {
            LogOnLevel(LogSeverityLevel.Information, message, exception);
        }

        /// <summary>
        /// Warning logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        public static void LogWarning(string message, Exception exception = null)
        {
            LogOnLevel(LogSeverityLevel.Warning, message, exception);
        }

        /// <summary>
        /// Error logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        public static void LogError(string message, Exception exception = null)
        {
            LogOnLevel(LogSeverityLevel.Error, message, exception);
        }

        /// <summary>
        /// Critical logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        public static void LogCritical(string message, Exception exception = null)
        {
            LogOnLevel(LogSeverityLevel.Critical, message, exception);
        }

        /// <summary>
        /// Safe logging of a message. Will check for errors, but never throw an exception. If the log can't be made with the chosen logger, a fallback log will be created.
        /// </summary>
        /// <param name="severityLevel">The severity level for this log.</param>
        /// <param name="message">The message to log (will be concatenated with any <paramref name="exception"/> information).</param>
        /// <param name="exception">Optional exception</param>
        public static void LogOnLevel(LogSeverityLevel severityLevel, string message, Exception exception = null)
        {
            LogInstanceInformation logInstanceInformation;
            try
            {
                var tenantValueProvider = new TenantConfigurationValueProvider();
                var correlationValueProvider = new CorrelationIdValueProvider();
                logInstanceInformation = new LogInstanceInformation
                {
                    ApplicationName = FulcrumApplication.Setup.Name,
                    ApplicationTenant = FulcrumApplication.Setup.Tenant,
                    RunTimeLevel = FulcrumApplication.Setup.RunTimeLevel,
                    ClientName = tenantValueProvider.CallingClientName,
                    ClientTenant = tenantValueProvider.Tenant,
                    CorrelationId = correlationValueProvider.CorrelationId,
                    TimeStamp = DateTimeOffset.Now,
                    SeverityLevel = severityLevel,
                    Message = message,
                    Exception = exception
                };
            }
            catch (Exception e)
            {
                var newMessage = message;
                if (exception != null) newMessage += $"\r{exception.Message}";
                logInstanceInformation = new LogInstanceInformation
                {
                    ApplicationName = FulcrumApplication.Setup.Name,
                    ApplicationTenant = FulcrumApplication.Setup.Tenant,
                    RunTimeLevel = FulcrumApplication.Setup.RunTimeLevel,
                    TimeStamp = DateTimeOffset.Now,
                    SeverityLevel = LogSeverityLevel.Critical,
                    Message = $"Logging failed when logging this:\r{newMessage}",
                    Exception = e
                };
            }
            LogInBackground(logInstanceInformation);
        }

        /// <summary>
        /// Safe logging of a message. Will check for errors, but never throw an exception. If the log can't be made with the chosen logger, a fallback log will be created.
        /// </summary>
        /// <param name="logInstanceInformation">Information about the logging.</param>
        private static void LogInBackground(LogInstanceInformation logInstanceInformation)
        {
            try
            {
                lock (ClassLock)
                {
                    if (_loggingInProgress)
                    {
                        FallbackToSimpleLoggingFailSafe("Recursive logging was interrupted.", logInstanceInformation);
                        return;
                    }
                }
                logInstanceInformation.StackTrace = Environment.StackTrace;
                ThreadHelper.FireAndForget(() => LogFailSafe(logInstanceInformation));
            }
            catch (Exception e)
            {
                FallbackToSimpleLoggingFailSafe("Failed to start background job for logging.", logInstanceInformation, e);
            }
        }

        /// <summary>
        /// Safe logging of a message. Will check for errors, but never throw an exception. If the log can't be made with the chosen logger, a fallback log will be created.
        /// </summary>
        /// <param name="logInstanceInformation">Information about the logging.</param>
        private static void LogFailSafe(LogInstanceInformation logInstanceInformation)
        {
            try
            {
                lock (ClassLock)
                {
                    if (_loggingInProgress)
                    {
                        FallbackToSimpleLoggingFailSafe("Log was is already in progress", logInstanceInformation);
                        return;
                    }
                    _loggingInProgress = true;
                }
                //ReSharper disable once ObjectCreationAsStatement
                new TenantConfigurationValueProvider
                {
                    Tenant = logInstanceInformation.ClientTenant,
                    CallingClientName = logInstanceInformation.ClientName
                };
                // ReSharper disable once ObjectCreationAsStatement
                new CorrelationIdValueProvider
                {
                    CorrelationId = logInstanceInformation.CorrelationId
                };
                var formattedMessage = FormatMessageFailSafe(logInstanceInformation);
                AlsoLogWithTraceSourceInDevelopment(logInstanceInformation.SeverityLevel, formattedMessage);
                LogWithConfiguredLoggerFailSafe(logInstanceInformation, formattedMessage);
            }
            catch (Exception e)
            {
                FallbackToSimpleLoggingFailSafe($"{nameof(LogFailSafe)} caught an exception.", logInstanceInformation, e);
            }
        }

        private static void LogWithConfiguredLoggerFailSafe(LogInstanceInformation logInstanceInformation,
            string formattedMessage)
        {
            try
            {
                if (FulcrumApplication.Setup.FullLogger != null)
                {
                    FulcrumApplication.Setup.FullLogger.LogAsync(logInstanceInformation).Wait();
                }
                else
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    FulcrumApplication.Setup.Logger.Log(logInstanceInformation.SeverityLevel, formattedMessage);
#pragma warning restore CS0618 // Type or member is obsolete
                }
            }
            catch (Exception e)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                IFulcrumLogger logger = FulcrumApplication.Setup.FullLogger ?? FulcrumApplication.Setup.Logger;
#pragma warning restore CS0618 // Type or member is obsolete
                FallbackToSimpleLoggingFailSafe(
                    $"{nameof(LogFailSafe)} caught an exception from logger {logger.GetType().FullName}.",
                    logInstanceInformation, e);
            }
        }

        private static void AlsoLogWithTraceSourceInDevelopment(LogSeverityLevel severityLevel, string formattedMessage)
        {
            if (!FulcrumApplication.IsInDevelopment) return;
#pragma warning disable CS0618 // Type or member is obsolete
            IFulcrumLogger logger = FulcrumApplication.Setup.FullLogger ?? FulcrumApplication.Setup.Logger;
#pragma warning restore CS0618 // Type or member is obsolete
            if (logger.GetType() == typeof(TraceSourceLogger)) return;
            TraceSourceLogger.Log(severityLevel, formattedMessage);
        }

        /// <summary>
        /// Create a formatted message based on <paramref name="exception"/>
        /// </summary>
        /// <param name="exception">The exception that we will create a log message for.</param>
        /// <returns>A formatted message, never null or empty.</returns>
        /// <remarks>This method should never throw an exception. If </remarks>
        [Obsolete("Use extension method ToLogString() for Exception")]
        public static string FormatMessageFailSafe(Exception exception)
        {
            if (exception == null) return "";
            return exception.ToLogString();
        }

        /// <summary>
        /// Create a formatted message based on <paramref name="logInstanceInformation"/>.
        /// </summary>
        /// <param name="logInstanceInformation">Information about the logging.</param>
        /// <returns>A formatted message, never null or empty</returns>
        public static string FormatMessageFailSafe(LogInstanceInformation logInstanceInformation)
        {
            if (logInstanceInformation == null) return null;
            try
            {
                return logInstanceInformation.ToLogString();
            }
            catch (Exception e)
            {
                return $"Formatting message failed ({e.Message}): {logInstanceInformation.Message}";
            }
        }
        

        /// <summary>
        /// Use this method to log when the original logging method fails.
        /// </summary>
        /// <param name="message">What went wrong with logging</param>
        /// <param name="logInstanceInformation">The message to log.</param>
        /// <param name="exception">If what went wrong had an exception</param>
        private static void FallbackToSimpleLoggingFailSafe(string message, LogInstanceInformation logInstanceInformation, Exception exception = null)
        {
            
            try
            {

                var totalMessage = message == null ? "" : $"{message}\r";
                if (logInstanceInformation != null)
                {
                    totalMessage += logInstanceInformation.ToLogString();
                }
                if (exception != null)
                {
                    totalMessage += $"\r\rException when logging\r{exception.ToLogString()}";
                }
                if (exception != null || (logInstanceInformation != null && logInstanceInformation.IsGreateThanOrEqualTo(LogSeverityLevel.Error)))
                {
                    totalMessage += $"\rStack trace up to when logging exception occured\r{Environment.StackTrace}";
                }
                try
                {
                    // If a message of warning or higher ends up here means it is critical, since this log will not end up in the normal log.
                    var severityLevel = logInstanceInformation == null 
                        ? LogSeverityLevel.Error 
                        : logInstanceInformation.IsGreateThanOrEqualTo(LogSeverityLevel.Warning) ? LogSeverityLevel.Critical : logInstanceInformation.SeverityLevel;
                    RecommendedForNetFramework.Log(severityLevel, totalMessage);
                }
                catch (Exception e)
                {
                    totalMessage += $"\r{e.ToLogString()}";
                    Debug.WriteLine($"{totalMessage}");
                }
            }
            catch (Exception)
            {
                // We give up
            }
        }
    }
}

