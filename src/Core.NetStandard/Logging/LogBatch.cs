using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
#pragma warning disable 659

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// Represents a log message with properties such as correlation id, calling client, severity and the text message.
    /// </summary>
    public class LogBatch : IValidatable, ILoggable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="firstLog">An optional first log record to add to <see cref="Records"/>.</param>
        public LogBatch(LogRecord firstLog = null)
        {
            Context = new LogContext();
            Records = new List<LogRecord>();
            if (firstLog != null) Records.Add(firstLog);
        }

        /// <summary>
        /// The context for the log.
        /// </summary>
        public LogContext Context { get; set; }

        /// <summary>
        /// The information about the individual log records.
        /// </summary>
        public List<LogRecord> Records { get; set; }

        /// <inheritdoc />
        public void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotNull(Context, nameof(Context), errorLocation);
            FulcrumValidate.IsValidated(Context, propertyPath, nameof(Context), errorLocation);
            FulcrumValidate.IsNotNull(Records, nameof(Records), errorLocation);
            FulcrumValidate.IsValidated(Records, propertyPath, nameof(Records), errorLocation);
        }

        /// <inheritdoc />
        public string ToLogString()
        {
            return ToLogString(true);
        }

        /// <summary>
        /// Summarize the information suitable for logging purposes.
        /// </summary>
        /// <param name="hideStackTrace">When this is true, any stack trace will be hidden.</param>
        public string ToLogString(bool hideStackTrace)
        {
            string result = null;
            foreach (var log in Records)
            {
                var line = log.ToLogString(hideStackTrace, Context);
                if (result == null) result = "";
                else result += "\r\r";
                result += line;
            }

            return result;
        }

        /// <summary>
        /// Returns the log with the highest severity level.
        /// </summary>
        public LogRecord GetLogWithHighestSeverityLevel()
        {
            return Records?.Aggregate((currentHighestLog, log) => log.IsGreateThanOrEqualTo(currentHighestLog.SeverityLevel) ? log : currentHighestLog);
        }

        internal void FilterByThreshold()
        {
            var threshold = FulcrumApplication.Setup.LogSeverityLevelThreshold;
            var logWithHighestSeverityLevel = GetLogWithHighestSeverityLevel();
            if (logWithHighestSeverityLevel.IsGreateThanOrEqualTo(FulcrumApplication.Setup.BatchLogAllSeverityLevelThreshold))
            {
                threshold = LogSeverityLevel.Verbose;
            }
            Records = Records.Where(log => log.IsGreateThanOrEqualTo(threshold)).ToList();
        }
    }
}
