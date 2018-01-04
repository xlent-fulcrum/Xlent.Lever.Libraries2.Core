using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;

namespace Xlent.Lever.Libraries2.Core.Health.Model
{
    /// <summary>
    /// Interface to be implemented by every controller for a service that should report their health.
    /// </summary>
    public interface IResourceHealth2
    {
        /// <summary>
        /// Get the health status for a specific <paramref name="tenant"/>.
        /// </summary>
        Task<HealthInfo> GetResourceHealthAsync(ITenant tenant);
    }
}
