using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// A convenience class for logging.
    /// </summary>
    public class ConsoleLogger : IFulcrumFullLogger
    {
        /// <inheritdoc />
        public void Log(LogSeverityLevel logSeverityLevel, string message)
        {
            try
            {
                if (logSeverityLevel == LogSeverityLevel.None) return;
                Console.WriteLine($"\r{logSeverityLevel} {message}\r");
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

