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

        /// <summary>
        /// Log <paramref name="message"/>.
        /// </summary>
        Task LogAsync(LogMessage message);
    }
}