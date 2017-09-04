using System;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Error.Logic;
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

        /// <summary>
        /// The chosen <see cref="IValueProvider"/> to use.
        /// </summary>
        /// <remarks>There are overrides for this, see e.g. in Xlent.Lever.Libraries2.WebApi.ContextValueProvider.</remarks>
        [Obsolete("Use FulcrumApplication.Setup.Logger", true)]
        public static IFulcrumLogger LoggerForApplication
        {
            get => FulcrumApplication.Setup.Logger;
            set => FulcrumApplication.Setup.Logger = value;
        }

        /// <summary>
        /// Recommended <see cref="IFulcrumLogger"/> for developing an application. For testenvironments and production, we recommend the Xlent.Lever.Logger capability.
        /// </summary>
        public static IFulcrumLogger RecommendedForNetFramework { get; } = TraceSourceLogger;

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
                logInstanceInformation = new LogInstanceInformation
                {
                    ApplicationName = FulcrumApplication.Setup.Name,
                    ApplicationTenant = FulcrumApplication.Setup.Tenant,
                    TimeStamp = DateTimeOffset.Now,
                    SeverityLevel = LogSeverityLevel.Critical,
                    Message = "Logging failed.",
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
            ThreadHelper.FireAndForget(() => SafeLog(logInstanceInformation));
        }

        /// <summary>
        /// Safe logging of a message. Will check for errors, but never throw an exception. If the log can't be made with the chosen logger, a fallback log will be created.
        /// </summary>
        /// <param name="logInstanceInformation">Information about the logging.</param>
        private static void SafeLog(LogInstanceInformation logInstanceInformation)
        {
            var formattedMessage = SafeFormatMessage(logInstanceInformation);
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
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
                var logger = FulcrumApplication.Setup.Logger;
                var fullLogger = logger as IFulcrumFullLogger;
                if (fullLogger != null)
                {
                    fullLogger.Log(logInstanceInformation);
                }
                else
                {
                    logger.Log(logInstanceInformation.SeverityLevel, formattedMessage);
                }
                AlsoLogWithTraceSourceInDevelopment(logInstanceInformation.SeverityLevel, formattedMessage);
            }
            catch (Exception e1)
            {
                try
                {
                    FallbackLoggingWhenAllElseFails($"{e1.Message}\r{formattedMessage}");
                }
                catch (Exception e2)
                {
                    try
                    {
                        Debug.WriteLine($"{e2.Message}\r{e1.Message}\r{formattedMessage}");
                    }
                    catch (Exception)
                    {
                        // We give up
                    }
                }
            }
        }

        private static void AlsoLogWithTraceSourceInDevelopment(LogSeverityLevel severityLevel, string formattedMessage)
        {
            if (!FulcrumApplication.IsInDevelopment) return;
            if (FulcrumApplication.Setup.Logger.GetType() == typeof(TraceSourceLogger)) return;
            TraceSourceLogger.Log(severityLevel, formattedMessage);
        }

        /// <summary>
        /// Create a formatted message based on <paramref name="message"/> and <paramref name="exception"/>
        /// </summary>
        /// <param name="timeStamp">The time when Log was called.</param>
        /// <param name="severityLevel">The severity level for this log.</param>
        /// <param name="message">The message. Can be null or empty if exception is not null.</param>
        /// <param name="exception">Optional exception</param>
        /// <returns>A formatted message, never null or empty</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null AND <paramref name="message"/> is null or empty.</exception>
        public static string SafeFormatMessage(DateTimeOffset timeStamp, LogSeverityLevel severityLevel, string message, Exception exception)
        {
            if (exception == null && string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }
            var contextInformation = GetContextInformation();
            var exceptionInformation = exception == null ? "" : $"\r{FormatMessage(exception)}";
            return $"--- {timeStamp} {severityLevel} {contextInformation}\r{message}{exceptionInformation}";
        }

        /// <summary>
        /// Create a formatted message based on <paramref name="message"/> and <paramref name="exception"/>
        /// </summary>
        /// <param name="logInstanceInformation">Information about the logging.</param>
        /// <returns>A formatted message, never null or empty</returns>
        public static string SafeFormatMessage(LogInstanceInformation logInstanceInformation)
        {
            try
            {
                var correlation = string.IsNullOrWhiteSpace(logInstanceInformation.CorrelationId)
                    ? ""
                    : $" {logInstanceInformation.CorrelationId}";
                var detailsLine =
                    $"{logInstanceInformation.TimeStamp}{correlation} {logInstanceInformation.SeverityLevel} {logInstanceInformation.ApplicationTenant} {logInstanceInformation.ApplicationName} ({logInstanceInformation.RunTimeLevel}) ";
                if (!string.IsNullOrWhiteSpace(logInstanceInformation.ClientName) || logInstanceInformation.ApplicationTenant != null)
                {
                    detailsLine += " client:";
                }
                if (!string.IsNullOrWhiteSpace(logInstanceInformation.ClientName))
                {
                    detailsLine += $" {logInstanceInformation.ClientName}";
                }
                if (logInstanceInformation.ApplicationTenant != null)
                {
                    detailsLine += $" {logInstanceInformation.ClientTenant}";
                }
                var exceptionLine = logInstanceInformation.Exception == null ? "" : $"\r{FormatMessage(logInstanceInformation.Exception)}";
                return $"{detailsLine}\r{logInstanceInformation.Message}{exceptionLine}";
            }
            catch (Exception e)
            {
                return $"Formatting message failed ({e.Message}): {logInstanceInformation.Message}";
            }
        }

        private static string GetContextInformation()
        {
            if (FulcrumApplication.Setup == null) return "";
            var result = FulcrumApplication.Setup.ToString();
            if (FulcrumApplication.Setup.ContextValueProvider == null) return result;
            var correlationIdProvider = new CorrelationIdValueProvider();
            var correlationId = correlationIdProvider.CorrelationId;
            result += string.IsNullOrWhiteSpace(correlationId) ? "" : $" CorrelationId {correlationId}";
            var tenantProvider = new TenantConfigurationValueProvider();
            var tenant = tenantProvider.Tenant;
            result += tenant == null ? "" : $" Tenant {tenant}";
            return $"result";
        }

        /// <summary>
        /// Create a formatted message based on <paramref name="exception"/>
        /// </summary>
        /// <param name="exception">The exception that we will create a log message for.</param>
        /// <returns>A formatted message, never null or empty.</returns>
        /// <remarks>This method should never throw an exception. If </remarks>
        public static string FormatMessage(Exception exception)
        {
            // This method should never fail, so if no exception was given, we will create an exception.
            try
            {
                InternalContract.RequireNotNull(exception, nameof(exception));
            }
            catch (Exception e)
            {
                exception = e;
            }
            var formatted = $"Exception type: {exception.GetType().FullName}";
            var fulcrumException = exception as FulcrumException;
            if (fulcrumException != null) formatted += $"\r{fulcrumException}";
            formatted += $"\rException message: {exception.Message}";
            formatted += $"\r{exception.StackTrace}";
            if (exception.InnerException != null)
            {
                formatted += $"\r--Inner exception--\r{FormatMessage(exception.InnerException)}";
            }
            return formatted;
        }


        /// <summary>
        /// Use this method to log when the original logging method fails.
        /// </summary>
        /// <param name="message">The original message to log.</param>
        private static void FallbackLoggingWhenAllElseFails(string message)
        {
            try
            {
                RecommendedForNetFramework.Log(LogSeverityLevel.Critical, message);
            }
            catch (Exception)
            {
                // This method must never fail.
            }
        }
    }
}

