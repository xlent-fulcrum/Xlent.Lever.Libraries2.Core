using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// Interface for basic logging
    /// </summary>
#pragma warning disable 618
    public interface IFulcrumFullLogger : IFulcrumLogger
#pragma warning restore 618
    {
        /// <summary>
        /// Log <paramref name="logs"/>.
        /// </summary>
        Task LogAsync(params LogInstanceInformation[] logs);
    }
}