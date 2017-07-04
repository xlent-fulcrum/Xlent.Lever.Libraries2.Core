using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;

namespace Xlent.Lever.Libraries2.Core.Platform.Configurations
{
    /// <summary>
    /// Contains what is needed in a convenience class for getting configurations and tokens.
    /// </summary>
    public interface ILeverServiceConfiguration
    {
        /// <summary>
        /// The name of the service that this service configuration is for.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// The tenant behind this running service
        /// </summary>
        ITenant ServiceTenant { get; }

        /// <summary>
        /// Gets the configuration for the current <see cref="ServiceTenant"/>.
        /// </summary>
        /// <returns></returns>
        Task<ILeverConfiguration> GetConfigurationAsync();

        /// <summary>
        /// Gets the configuration for another tenant.
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        Task<ILeverConfiguration> GetConfigurationForAsync(ITenant tenant);
    }
}