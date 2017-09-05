using System;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// Represents a log message with properties such as correlation id, calling client, severity and the text message.
    /// </summary>
    public class LogInstanceInformation : IValidatable
    {
        /// <summary>
        /// The name of the application that was executing when the log message was created.
        /// Mandatory.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The tenant that the application belongs to.
        /// Mandatory.
        /// </summary>
        public ITenant ApplicationTenant { get; set; }

        /// <summary>
        /// The name of the calling client.
        /// Optional.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// The tenant that the client belongs to. This is for multi tenant applications. For single tenant applications, this property should be null.
        /// Optional.
        /// </summary>
        public ITenant ClientTenant { get; set; }
        
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
        /// The <see cref="RunTimeLevelEnum"/>, e.g. Develop, Test, etc.
        /// </summary>
        public RunTimeLevelEnum RunTimeLevel { get; set; }

        /// <summary>
        /// The call stack for the moment when the logging was turned into it's own thread.
        /// </summary>
        public string StackTrace { get; set; }

        /// <inheritdoc />
        public void Validate(string errorLocation, string propertyPath = "")
        {
            // Note! Don't check Org/Env here, since they can be null for yet-to-be-discovered reasons
            FulcrumValidate.IsNotNullOrWhiteSpace(ApplicationName, nameof(ApplicationName), errorLocation);
            FulcrumValidate.IsNotNull(ApplicationTenant, nameof(ApplicationTenant), errorLocation);
            FulcrumValidate.IsValidated(ApplicationTenant, propertyPath, nameof(ApplicationTenant), errorLocation);
            if (ClientName != null) FulcrumValidate.IsNotNullOrWhiteSpace(ClientName, nameof(ClientName), errorLocation);
            if (ClientTenant != null) FulcrumValidate.IsValidated(ClientTenant, propertyPath, nameof(ClientTenant), errorLocation);
            FulcrumValidate.IsNotDefaultValue(TimeStamp, nameof(TimeStamp), errorLocation);
            FulcrumValidate.IsLessThanOrEqualTo(DateTimeOffset.Now, TimeStamp, nameof(TimeStamp), errorLocation);
            FulcrumValidate.IsNotDefaultValue(SeverityLevel, nameof(SeverityLevel), errorLocation);
            FulcrumValidate.IsNotNullOrWhiteSpace(Message, nameof(Message), errorLocation);
            FulcrumValidate.IsNotDefaultValue(RunTimeLevel, nameof(RunTimeLevel), errorLocation);
        }
    }
}
