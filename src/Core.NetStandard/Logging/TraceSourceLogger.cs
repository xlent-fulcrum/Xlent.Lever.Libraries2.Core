using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// A convenience class for logging.
    /// </summary>
    public class TraceSourceLogger : IFulcrumFullLogger
    {
        private static readonly TraceSource TraceSource = new TraceSource("FulcrumTraceSource");

        /// <inheritdoc />
        public void Log(LogSeverityLevel logSeverityLevel, string message)
        {
            try
            {
                if (logSeverityLevel == LogSeverityLevel.None) return;
                TraceEventType eventType;
                switch (logSeverityLevel)
                {
                    case LogSeverityLevel.Verbose:
                        eventType = TraceEventType.Verbose;
                        break;
                    case LogSeverityLevel.Information:
                        eventType = TraceEventType.Information;
                        break;
                    case LogSeverityLevel.Warning:
                        eventType = TraceEventType.Warning;
                        break;
                    case LogSeverityLevel.Error:
                        eventType = TraceEventType.Error;
                        break;
                    case LogSeverityLevel.Critical:
                        eventType = TraceEventType.Critical;
                        break;
                    // ReSharper disable once RedundantCaseLabel
                    case LogSeverityLevel.None:
                    default:
                        TraceSource.TraceEvent(TraceEventType.Critical, 0,
                            $"Unexpected {nameof(logSeverityLevel)} ({logSeverityLevel}) for message: {message}.");
                        return;
                }
                TraceSource.TraceEvent(eventType, 0, $"\r{message}\r");
            }
            catch (Exception)
            {
                // This method must never fail.
            }
        }

        /// <inheritdoc />
        public Task LogAsync(LogBatch logBatch)
        {
            if (logBatch?.Records == null) return Task.CompletedTask;
            foreach (var log in logBatch.Records)
            {
                Log(log.SeverityLevel, Logging.Log.FormatMessageFailSafe(logBatch, log));
            }

            return Task.CompletedTask;
        }
    }
}

