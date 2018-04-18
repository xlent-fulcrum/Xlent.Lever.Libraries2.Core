using System;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Health.Model;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;

namespace Xlent.Lever.Libraries2.Core.Health.Logic
{
    /// <summary>
    /// Knows the logic behind aggregating health of many resources.
    /// </summary>
    public class ResourceHealthAggregator
    {
        /// <summary>
        /// The current Tenant
        /// </summary>
        public Tenant Tenant { get; }
        private int _warnings;
        private int _errors;
        private readonly HealthResponse _healthResponse;
        private string _lastErrorMessage;
        private string _lastWarningMessage;

        /// <summary>
        /// The signature for a resource health method.
        /// </summary>
        /// <returns></returns>
        public delegate Task<HealthResponse> GetResourceHealthDelegate(Tenant tenant);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tenant">The tenant that we should focus on.</param>
        /// <param name="resourceName">The name of the resource.</param>
        public ResourceHealthAggregator(Tenant tenant, string resourceName)
        {
            Tenant = tenant;
            _healthResponse = new HealthResponse(resourceName);
        }

        /// <summary>
        /// Check the health of a specific resource and aggregate it to the complete health state.
        /// </summary>
        /// <param name="resourceName">The name to use for the resource</param>
        /// <param name="resource">A resource that we want to get the health for and add it to the aggregated health.</param>
        public async Task AddResourceHealthAsync(string resourceName, IResourceHealth resource)
        {
            await AddResourceHealthAsync(resourceName, resource.GetResourceHealthAsync);
        }

        /// <summary>
        /// Call e healt check delegate and aggregate the answer to the complete health state.
        /// </summary>
        /// <param name="resourceName">The name to use for the resource</param>
        /// <param name="healthDelegate">A method that returns a health, that we will add to the aggregated health.</param>
        public async Task AddResourceHealthAsync(string resourceName, GetResourceHealthDelegate healthDelegate)
        {
            HealthResponse response;
            try
            {
                response = await healthDelegate(Tenant);
                if (string.IsNullOrWhiteSpace(response.Resource)) response.Resource = resourceName;
            }
            catch (Exception e)
            {
                response = new HealthResponse(resourceName)
                {
                    Status = HealthResponse.StatusEnum.Error,
                    Message = e.Message
                };
            }
            AddHealthResponse(response);
        }

        /// <summary>
        /// Add a health response and aggregate it to the complete health state.
        /// </summary>
        /// <param name="healthResponse"></param>
        public void AddHealthResponse(HealthResponse healthResponse)
        {
            _healthResponse.Resources.Add(healthResponse);
            if (healthResponse.Status == HealthResponse.StatusEnum.Warning)
            {
                _warnings++;
                _lastWarningMessage = healthResponse.Message;
            }
            if (healthResponse.Status == HealthResponse.StatusEnum.Error)
            {
                _errors++;
                _lastErrorMessage = healthResponse.Message;
            }
        }

        /// <summary>
        /// Get the aggregated health.
        /// </summary>
        /// <returns></returns>
        public HealthResponse GetAggregatedHealthResponse()
        {
            if (_errors > 0)
            {
                _healthResponse.Status = HealthResponse.StatusEnum.Error;
                _healthResponse.Message = _errors == 1 ? _lastErrorMessage : $"Found {_errors} errors and {_warnings} warnings.";
            }
            else if (_warnings > 0)
            {
                _healthResponse.Status = HealthResponse.StatusEnum.Warning;
                _healthResponse.Message = _errors == 1 ? _lastWarningMessage : $"Found {_warnings} warnings.";
            }
            else
            {
                _healthResponse.Status = HealthResponse.StatusEnum.Ok;
                _healthResponse.Message = "OK";
            }
            return _healthResponse;
        }
    }
}
