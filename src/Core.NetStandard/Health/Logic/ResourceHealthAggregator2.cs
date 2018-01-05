﻿using System;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Health.Model;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;

namespace Xlent.Lever.Libraries2.Core.Health.Logic
{
    /// <summary>
    /// Knows the logic behind aggregating health of many resources.
    /// </summary>
    public class ResourceHealthAggregator2
    {
        /// <summary>
        /// The current Tenant
        /// </summary>
        public ITenant Tenant { get; }
        private int _warnings;
        private int _errors;
        private readonly Model.Health _health;
        private string _lastErrorMessage;
        private string _lastWarningMessage;

        /// <summary>
        /// The signature for a resource health method.
        /// </summary>
        /// <returns></returns>
        public delegate Task<HealthInfo> GetResourceHealthDelegate(ITenant tenant);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tenant">The tenant that we should focus on.</param>
        /// <param name="resourceName">The name of the resource.</param>
        public ResourceHealthAggregator2(ITenant tenant, string resourceName)
        {
            Tenant = tenant;
            _health = new Model.Health(resourceName);
        }

        /// <summary>
        /// Check the health of a specific resource and aggregate it to the complete health state.
        /// </summary>
        /// <param name="resourceName">The name to use for the resource</param>
        /// <param name="resource">A resource that we want to get the health for and add it to the aggregated health.</param>
        public async Task AddResourceHealthAsync(string resourceName, IResourceHealth2 resource)
        {
            await AddResourceHealthAsync(resourceName, resource.GetResourceHealthAsync);
        }

        /// <summary>
        /// Call a health check delegate and aggregate the answer to the complete health state.
        /// </summary>
        /// <param name="resourceName">The name to use for the resource</param>
        /// <param name="healthDelegate">A method that returns a health, that we will add to the aggregated health.</param>
        public async Task AddResourceHealthAsync(string resourceName, GetResourceHealthDelegate healthDelegate)
        {
            HealthInfo response;
            try
            {
                response = await healthDelegate(Tenant);
                //Check this?
                if (string.IsNullOrWhiteSpace(response.Resource)) response.Resource = resourceName;
            }
            catch (Exception e)
            {
                response = new HealthInfo(resourceName)
                {
                    Status = HealthInfo.StatusEnum.Error,
                    Message = e.Message
                };
            }
            AddHealthResponse(response);
        }

        /// <summary>
        /// Add a health response and aggregate it to the complete health state.
        /// </summary>
        /// <param name="healthInfo"></param>
        public void AddHealthResponse(HealthInfo healthInfo)
        {
            _health.Resources.Add(healthInfo);
            if (healthInfo.Status == HealthInfo.StatusEnum.Warning)
            {
                _warnings++;
                _lastWarningMessage = healthInfo.Message;
            }
            if (healthInfo.Status == HealthInfo.StatusEnum.Error)
            {
                _errors++;
                _lastErrorMessage = healthInfo.Message;
            }
        }

        /// <summary>
        /// Get the aggregated health.
        /// </summary>
        /// <returns></returns>
        public Model.Health GetAggregatedHealthResponse()
        {
            return _health;
        }
    }
}