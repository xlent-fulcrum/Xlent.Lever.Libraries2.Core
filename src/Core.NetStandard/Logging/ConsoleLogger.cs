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
        private static readonly TraceSource TraceSource = new TraceSource("FulcrumTraceSource");

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
        public async Task LogAsync(LogInstanceInformation logInformation)
        {
            Log(logInformation.SeverityLevel, Logging.Log.FormatMessageFailSafe(logInformation));
            await Task.Yield();
        }
    }
}

