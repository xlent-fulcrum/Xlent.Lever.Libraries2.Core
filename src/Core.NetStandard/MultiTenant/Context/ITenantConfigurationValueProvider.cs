using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Platform.Configurations;

namespace Xlent.Lever.Libraries2.Core.MultiTenant.Context
{
    /// <summary>
    /// Adds Tenant and LeverConfiguration to what <see cref="ICorrelationIdValueProvider"/> provides.
    /// </summary>
    public interface ITenantConfigurationValueProvider : ICorrelationIdValueProvider
    {
        /// <summary>
        /// The current Tenant.
        /// </summary>
        ITenant Tenant { get; set; }

        /// <summary>
        /// The current configuration.
        /// </summary>
        ILeverConfiguration LeverConfiguration { get; set; }
    }
}