﻿using System;
using System.Collections.Generic;

namespace Xlent.Lever.Libraries2.Core.Health.Model
{
    public class HealthInfo
    {
        /// </summary>
        public enum StatusEnum
        {
            /// <summary>
            /// Everything is OK for the service - no minor or major problems.
            /// </summary>
            Ok = 0,
            /// <summary>
            /// The service/resource have no major problems, but at least one minor problems, such as longer response times than expected.
            /// </summary>
            Warning = 1,
            /// <summary>
            /// The service/resource have at least one major problem, such as no connection to an important resource.
            /// </summary>
            Error = 2
        }

        /// <summary>
        /// Constructor. Will set the status to OK initially.
        /// </summary>
        public HealthInfo()
        {
            Status = StatusEnum.Ok;
        }

        /// <summary>
        /// Constructor. Will set the status to OK initially.
        /// </summary>
        /// <param name="resourceName">The name of the resource that this health response is for.</param>
        public HealthInfo(string resourceName)
            :this()
        {
            Resource = resourceName;
        }

        /// <summary>
        /// A message that describes the status.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Summary status for the service or resource.
        /// </summary>
        public StatusEnum Status { get; set; }

        /// <summary>
        ///  The name of the service/resource
        /// </summary>
        public string Resource { get; set; }
    }
}