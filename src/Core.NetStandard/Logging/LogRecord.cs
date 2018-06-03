using System;
using System.Runtime.CompilerServices;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// Represents a log message with properties such as correlation id, calling client, severity and the text message.
    /// </summary>
    public class LogRecord : IValidatable, ILoggable
    {
        public LogRecord()
        {

        }
        internal LogRecord(
            LogSeverityLevel severityLevel,
            string message,
            Exception exception,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            var correlationValueProvider = new CorrelationIdValueProvider();

            CorrelationId = correlationValueProvider.CorrelationId;
            TimeStamp = DateTimeOffset.Now;
            SeverityLevel = severityLevel;
            Message = message;
            Location = $"{memberName} in {filePath} line {lineNumber}";
            Exception = exception;

            if (IsGreateThanOrEqualTo(LogSeverityLevel.Error))
            {
                StackTrace = Environment.StackTrace;
            }
        }
        /// <summary>
        /// The time that the log message was created
        /// Mandatory, i.e. must not be the default value.
        /// </summary>
        public DateTimeOffset TimeStamp { get; set; }

        /// <summary>
        /// A correlation id that ties this log message together in different systems or null.
        /// Optional.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// The <see cref="LogSeverityLevel"/> of the log message
        /// </summary>
        public LogSeverityLevel SeverityLevel { get; set; }

        /// <summary>
        /// The logged message in plain text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Information about an exception behind the message.
        /// Optional.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Where the log was issued (typically file name and line number)
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The call stack for the moment when the logging was turned into it's own thread.
        /// </summary>
        public string StackTrace { get; set; }

        /// <inheritdoc />
        public void Validate(string errorLocation, string propertyPath = "")
        {
            // Note! Don't check Org/Env here, since they can be null for yet-to-be-discovered reasons
            FulcrumValidate.IsNotDefaultValue(TimeStamp, nameof(TimeStamp), errorLocation);
            //FulcrumValidate.IsLessThanOrEqualTo(DateTimeOffset.Now, TimeStamp, nameof(TimeStamp), errorLocation);
            FulcrumValidate.IsNotDefaultValue(SeverityLevel, nameof(SeverityLevel), errorLocation);
            FulcrumValidate.IsNotNullOrWhiteSpace(Message, nameof(Message), errorLocation);
        }

        /// <inheritdoc />
        public override string ToString() => $"{SeverityLevel}: {Message}";

        /// <inheritdoc />
        public string ToLogString()
        {
            return ToLogString(true);
        }

        /// <summary>
        /// Summarize the information suitable for logging purposes.
        /// </summary>
        /// <param name="hideStackTrace">When this is true, any stack trace will be hidden.</param>
        /// <param name="logContext">Information from <see cref="LogContext"/>.</param>
        public string ToLogString(bool hideStackTrace, LogContext logContext = null)
        {
            var correlation = string.IsNullOrWhiteSpace(CorrelationId)
                ? ""
                : $" {CorrelationId}";
            var detailsLine =
                $"{TimeStamp.ToLogString()}{correlation} {SeverityLevel}";
            var context = logContext?.ToLogString();
            if (!string.IsNullOrWhiteSpace(context)) detailsLine += $" {context}";
            var exceptionLine = "";
            var stackTraceLine = "";
            if (Exception != null) exceptionLine = $"\r{Exception.ToLogString(hideStackTrace)}";
            if (!hideStackTrace && StackTrace != null && (Exception != null || IsGreateThanOrEqualTo(LogSeverityLevel.Error)))
            {
                stackTraceLine = $"\r{StackTrace}";
            }
            return $"{detailsLine}\r{Message}{exceptionLine}{stackTraceLine}";
        }

        /// <summary>
        /// Compares the current <see cref="SeverityLevel"/> with the supplied <paramref name="severityLevel"/>.
        /// </summary>
        /// <returns>True if the current level is greater than or equal to the value in the parameter <paramref name="severityLevel"/>.</returns>
        public bool IsGreateThanOrEqualTo(LogSeverityLevel severityLevel)
        {
            return (int)SeverityLevel >= (int)severityLevel;
        }
    }
}
