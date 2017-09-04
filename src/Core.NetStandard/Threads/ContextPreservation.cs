using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Platform.Configurations;

namespace Xlent.Lever.Libraries2.Core.Threads
{
    internal class ContextPreservation
    {
        private readonly ITenant _tenant;
        private readonly string _correlationId;
        private readonly string _callingClientName;
        private readonly ILeverConfiguration _configuration;

        public ContextPreservation()
        {
            var tenantProvider = new TenantConfigurationValueProvider();
            _tenant = tenantProvider.Tenant;
            _callingClientName = tenantProvider.CallingClientName;
            _configuration = tenantProvider.LeverConfiguration;
            var correlationProvider = new CorrelationIdValueProvider();
            _correlationId = correlationProvider.CorrelationId;
        }

        public void PreserveContext(Action<CancellationToken> action, CancellationToken cancellationToken)
        {
            RestoreContext();
            action(cancellationToken);
        }

        internal void PreserveContext(Action action)
        {
            RestoreContext();
            action();
        }

        private void RestoreContext()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new TenantConfigurationValueProvider
            {
                Tenant = _tenant,
                CallingClientName = _callingClientName,
                LeverConfiguration = _configuration
            };
            // ReSharper disable once ObjectCreationAsStatement
            new CorrelationIdValueProvider
            {
                CorrelationId = _correlationId
            };
        }
    }
}
