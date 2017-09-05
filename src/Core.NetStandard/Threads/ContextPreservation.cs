using System;
using System.Threading;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.MultiTenant.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Platform.Configurations;

namespace Xlent.Lever.Libraries2.Core.Threads
{
    internal class ContextPreservation
    {
        private readonly ITenant _clientTenant;
        private readonly string _correlationId;
        private readonly string _callingClientName;
        private readonly ILeverConfiguration _configuration;

        public ContextPreservation()
        {
            var tenantProvider = new TenantConfigurationValueProvider();
            _clientTenant = tenantProvider.Tenant;
            _callingClientName = tenantProvider.CallingClientName;
            _configuration = tenantProvider.LeverConfiguration;
            var correlationProvider = new CorrelationIdValueProvider();
            _correlationId = correlationProvider.CorrelationId;
        }

        public void ExecuteActionFailSafe(Action<CancellationToken> action, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                RestoreContext();
                action(cancellationToken);
            }
            catch (Exception e)
            {
                try
                {
                    var logger = new TraceSourceLogger();
                    logger.Log(LogSeverityLevel.Critical, $"Background thread failed {e.Message}.\rApplication information: {FulcrumApplication.ToLogString()}\rContext information: {ToLogString()}");
                }
                catch (Exception)
                {
                    // Give up
                }
            }
        }

        private void RestoreContext()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new TenantConfigurationValueProvider
            {
                Tenant = _clientTenant,
                CallingClientName = _callingClientName,
                LeverConfiguration = _configuration
            };
            // ReSharper disable once ObjectCreationAsStatement
            new CorrelationIdValueProvider
            {
                CorrelationId = _correlationId
            };
        }

        public string ToLogString()
        {
            return $"Client: {_callingClientName} {_clientTenant} CorrelationId: {_correlationId}";
        }
    }
}
