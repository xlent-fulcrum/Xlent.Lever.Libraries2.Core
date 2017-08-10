using System;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Logging.Model;

namespace Xlent.Lever.Libraries2.Core.Logging.Logic
{
    /// <summary>
    /// A convenience class for logging.
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// Safe logging of a message. Will check for errors, but never throw an exception. If the log can't be made, a fallback log will be created.
        /// </summary>
        /// <param name="logger">The logger to use for publishing the message.</param>
        /// <param name="severityLevel">The severity level for this log.</param>
        /// <param name="message">The message to log (will be concatenated with any <paramref name="exception"/> information).</param>
        /// <param name="exception">Optional exception</param>
        public static async Task LogAsync(IFulcrumLogger logger, LogSeverityLevel severityLevel, string message, Exception exception = null)
        {
            try
            {
                InternalContract.RequireNotNull(logger, nameof(logger));
                var formattedMessage = FormatMessage(message, exception);
                await logger.LogAsync(severityLevel, formattedMessage);
            }
            catch (Exception)
            {
                // TODO: Log somewhere
                //var formattedMessage = FormatMessage(message, e);
                //await SafeLogger.Instance.LogAsync(tenant, SeverityLevel.Critical, formattedMessage);
            }
        }

        /// <summary>
        /// Create a formatted message based on <paramref name="message"/> and <paramref name="exception"/>
        /// </summary>
        /// <param name="message">The message. Can be null or empty if exception is not null.</param>
        /// <param name="exception">Optional exception</param>
        /// <returns>A formatted message, never null or empty</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null AND <paramref name="message"/> is null or empty.</exception>
        public static string FormatMessage(string message, Exception exception)
        {
            if (exception == null && string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }
            return exception != null ? FormatMessage(exception) : message;
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
    }
}
