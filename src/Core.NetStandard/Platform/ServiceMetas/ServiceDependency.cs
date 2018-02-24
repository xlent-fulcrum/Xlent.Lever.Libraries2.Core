namespace Xlent.Lever.Libraries2.Core.Platform.ServiceMetas
{
    /// <summary>
    /// Describes the relation this service has to another service
    /// </summary>
    public class ServiceDependency
    {
        /// <summary>
        /// The other service' /ServiceMetas url
        /// </summary>
        public string ServiceMetasUrl { get; set; }
    }
}
