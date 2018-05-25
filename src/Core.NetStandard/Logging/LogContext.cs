﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
#pragma warning disable 659

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// Represents a log message with properties such as correlation id, calling client, severity and the text message.
    /// </summary>
    public class LogContext : IValidatable, ILoggable
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
        public Tenant ApplicationTenant { get; set; }

        /// <summary>
        /// The name of the calling client.
        /// Optional.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// The tenant that the client belongs to. This is for multi tenant applications. For single tenant applications, this property should be null.
        /// Optional.
        /// </summary>
        public Tenant ClientTenant { get; set; }

        /// <summary>
        /// A correlation id that ties this log message together in different systems or null.
        /// Optional.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// The <see cref="RunTimeLevelEnum"/>, e.g. Develop, Test, etc.
        /// </summary>
        public RunTimeLevelEnum RunTimeLevel { get; set; }

        /// <summary>
        /// The information about the individual log records.
        /// </summary>
        public List<LogRecord> IndividualLogs { get; set; }

        /// <inheritdoc />
        public void Validate(string errorLocation, string propertyPath = "")
        {
            // Note! Don't check Org/Env here, since they can be null for yet-to-be-discovered reasons
            FulcrumValidate.IsNotNullOrWhiteSpace(ApplicationName, nameof(ApplicationName), errorLocation);
            FulcrumValidate.IsNotNull(ApplicationTenant, nameof(ApplicationTenant), errorLocation);
            FulcrumValidate.IsValidated(ApplicationTenant, propertyPath, nameof(ApplicationTenant), errorLocation);
            if (ClientName != null) FulcrumValidate.IsNotNullOrWhiteSpace(ClientName, nameof(ClientName), errorLocation);
            if (ClientTenant != null) FulcrumValidate.IsValidated(ClientTenant, propertyPath, nameof(ClientTenant), errorLocation);
            //FulcrumValidate.IsLessThanOrEqualTo(DateTimeOffset.Now, TimeStamp, nameof(TimeStamp), errorLocation);
            FulcrumValidate.IsNotDefaultValue(RunTimeLevel, nameof(RunTimeLevel), errorLocation);
            FulcrumValidate.IsValidated(IndividualLogs, propertyPath, nameof(IndividualLogs), errorLocation);
        }

        /// <inheritdoc />
        public string ToLogString()
        {
            var contextInfo = $"{ApplicationTenant} {ApplicationName} ({RunTimeLevel})";

            if (!string.IsNullOrWhiteSpace(ClientName))
            {
                contextInfo += $" client: {ClientName}";

                if (ClientTenant != null)
                {
                    contextInfo += $" {ClientTenant.ToLogString()}";
                }
            }

            return contextInfo;
        }

        /// <summary>
        /// Returns the log with the highest severity level.
        /// </summary>
        public LogRecord GetLogWithHighestSeverityLevel()
        {
            return IndividualLogs?.Aggregate((currentHighestLog, log) => log.IsGreateThanOrEqualTo(currentHighestLog.SeverityLevel) ? log : currentHighestLog);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is LogContext other)) return false;
            return Equals(ClientTenant, other.ClientTenant)
                   && ClientName == other.ClientName
                   && Equals(ApplicationTenant, other.ApplicationTenant)
                   && ApplicationName == other.ApplicationName
                   && CorrelationId == other.CorrelationId
                   && RunTimeLevel == other.RunTimeLevel;
        }

        internal void FilterByThreshold()
        {
            var threshold = FulcrumApplication.Setup.LogSeverityLevelThreshold;
            var logWithHighestSeverityLevel = GetLogWithHighestSeverityLevel();
            if (logWithHighestSeverityLevel.IsGreateThanOrEqualTo(FulcrumApplication.Setup.BatchLogAllSeverityLevelThreshold))
            {
                threshold = LogSeverityLevel.Verbose;
            }
            IndividualLogs = IndividualLogs.Where(log => log.IsGreateThanOrEqualTo(threshold)).ToList();
        }
    }
}
