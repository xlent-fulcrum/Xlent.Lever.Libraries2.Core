using System;
using System.Collections.Generic;

namespace Xlent.Lever.Libraries2.Core.Health.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Health
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Health(string name)
        {
            Name = name;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Timestamp of the healthresponse
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// The healthresponse of the sevices subresources
        /// </summary>
        public List<HealthInfo> Resources { get; set; }

    }
}