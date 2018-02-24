using System;
using System.Collections.Generic;

namespace Xlent.Lever.Libraries2.Core.Health.Model
{
    /// <summary>
    /// The response when calling a service at /ServiceMetas/Health
    /// </summary>
    public class Health
    {
        /// <summary>
        /// Create a Health with the service name
        /// </summary>
        public Health(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the service
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Timestamp of the health response
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Summary status for the whole service.
        /// </summary>
        public HealthInfo.StatusEnum Status { get; set; }

        /// <summary>
        /// A message that describes the status of the whole service.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The health info of the service' subresources
        /// </summary>
        public List<HealthInfo> Resources { get; } = new List<HealthInfo>();

    }
}