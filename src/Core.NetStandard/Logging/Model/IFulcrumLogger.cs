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
        Task LogAsync(LogSeverityLevel logSeverityLevel, string message);

        // TODO: Remove after next update of all Fulcrum repositories.
        /// <summary>
        /// Log <paramref name="exception"/>.
        /// </summary>
        [Obsolete("Use LogAsync(LogSeverityLevel, string).", true)]
        Task LogAsync(Exception exception);

        // TODO: Remove after next update of all Fulcrum repositories.
        /// <summary>
        /// Log <paramref name="message"/>.
        /// </summary>
        [Obsolete("Use LogAsync(LogSeverityLevel, string).", true)]
        Task LogAsync(LogMessage message);
    }
}