using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// Interface for basic logging
    /// </summary>
    public interface IFulcrumFullLogger : IFulcrumLogger
    {
        /// <summary>
        /// Log <paramref name="message"/>.
        /// </summary>
        Task LogAsync(LogInstanceInformation message);
    }
}