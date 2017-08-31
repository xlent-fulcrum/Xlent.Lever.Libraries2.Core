using System;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Logging.Model
{
    /// <summary>
    /// Interface for basic logging
    /// </summary>
    public interface IFulcrumLogger
    {
        /// <summary>
        /// Log <paramref name="message"/> with level <paramref name="logSeverityLevel"/>.
        /// </summary>
        [Obsolete("Use the synchronous version")]
        Task LogAsync(LogSeverityLevel logSeverityLevel, string message);

        /// <summary>
        /// Log <paramref name="exception"/>.
        /// </summary>
        [Obsolete("Use Log(LogSeverityLevel, string).")]
        Task LogAsync(Exception exception);

        /// <summary>
        /// Log <paramref name="message"/>.
        /// </summary>
        [Obsolete("Use Log(LogSeverityLevel, string).")]
        Task LogAsync(LogMessage message);

        /// <summary>
        /// Log <paramref name="message"/> with level <paramref name="logSeverityLevel"/>.
        /// </summary>
        void Log(LogSeverityLevel logSeverityLevel, string message);
    }
}