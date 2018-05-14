using Xlent.Lever.Libraries2.Core.MultiTenant.Model;

namespace Xlent.Lever.Libraries2.Core.Platform.ServiceMetas
{
    /// <inheritdoc />
    public class ServiceInformationWithTenant : ServiceInformation
    {
        /// <summary>
        /// The <see cref="Tenant"/> running the service itself.
        /// </summary>
        public Tenant Tenant { get; set; }
    }
}
