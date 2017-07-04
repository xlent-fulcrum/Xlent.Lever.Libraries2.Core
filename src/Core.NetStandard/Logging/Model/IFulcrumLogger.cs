using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;

namespace Xlent.Lever.Libraries2.Core.Logging.Model
{
    /// <summary>
    /// Interface for basic logging
    /// </summary>
    public interface IFulcrumLogger
    {
        /// <summary>
        /// Log <paramref name="message"/> for <paramref name="tenant"/> with level <paramref name="logSeverityLevel"/>.
        /// </summary>
        /// <remarks>Same as <see cref="Log"/>, but asynchronous.</remarks>
        Task LogAsync(ITenant tenant, LogSeverityLevel logSeverityLevel, string message);

        /// <summary>
        /// Log <paramref name="message"/> for <paramref name="tenant"/> with level <paramref name="logSeverityLevel"/>.
        /// </summary>
        /// <remarks>Same as <see cref="LogAsync"/>, but synchronous.</remarks>
        void Log(ITenant tenant, LogSeverityLevel logSeverityLevel, string message);
    }
}