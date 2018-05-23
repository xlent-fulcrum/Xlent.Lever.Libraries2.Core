﻿using System;
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
        public Task LogAsync(params LogInstanceInformation[] logs)
        {
            if (logs == null) return Task.CompletedTask;
            foreach (var log in logs)
            {
                Log(log.SeverityLevel, Logging.Log.FormatMessageFailSafe(log));
            }
            return Task.CompletedTask;
        }
    }
}

