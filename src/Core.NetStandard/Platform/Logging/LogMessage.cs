using System;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Logging.Model;

namespace Xlent.Lever.Libraries2.Core.Platform.Logging
{
    /// <summary>
    /// Represents a log message with properties such as correlation id, calling client, severity and the text message.
    /// </summary>
    public class LogMessage : IValidatable
    {
        /// <summary>
        /// Tenant.Organization
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Tenant.Environment
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// The name of the service that the logging is done from
        /// </summary>
        public string Originator { get; set; }

        /// <summary>
        /// The name of the calling system
        /// </summary>
        public string CallingClientName { get; set; }

        /// <summary>
        /// A correlation id that ties this log message together in different systems
        /// </summary>
        public string CorrelationId { get; set; }

        // TODO: No need to be specific about that it is UTC? Name change?
        /// <summary>
        /// Timestamp in UTC when the log message was created
        /// </summary>
        public DateTimeOffset UtcDateTimeOffset { get; set; }

        /// <summary>
        /// The <see cref="LogSeverityLevel"/> of the log message
        /// </summary>
        public LogSeverityLevel SeverityLevel { get; set; }

        /// <summary>
        /// The logged message in plain text
        /// </summary>
        public string Message { get; set; }

        /// <inheritdoc />
        public void Validate(string errorLocation, string propertyPath = "")
        {
            // Note! Don't check Org/Env here, since they can be null for yet-to-be-discovered reasons
            FulcrumValidate.IsNotNullOrWhiteSpace(Originator, nameof(Originator), errorLocation);
            FulcrumValidate.IsNotNull(UtcDateTimeOffset, nameof(UtcDateTimeOffset), errorLocation);
            FulcrumValidate.IsNotDefaultValue(UtcDateTimeOffset, nameof(UtcDateTimeOffset), errorLocation);
            FulcrumValidate.IsNotNull(SeverityLevel, nameof(SeverityLevel), errorLocation);
        }
    }
}
