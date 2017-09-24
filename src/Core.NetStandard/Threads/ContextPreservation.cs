using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.MultiTenant.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Platform.Configurations;

namespace Xlent.Lever.Libraries2.Core.Threads
{
    /// <summary>
    /// A way to preserve important properties when starting new threads/jobs.
    /// </summary>
    public class ContextPreservation : ILoggable
    {
        private const int MaxDepthForBackgroundThreads = 5;
        private readonly ITenant _clientTenant;
        private readonly string _correlationId;
        private readonly string _callingClientName;
        private readonly ILeverConfiguration _configuration;
        private readonly List<string> _stackTraces;
        private readonly int _callDepth;

        [ThreadStatic]
        private static int _threadCallDepth;
        [ThreadStatic]
        private static List<string> _threadStackTraces;

        private static readonly object ClassLock = new object();

        /// <summary></summary>
        public ContextPreservation()
        {
            lock (ClassLock)
            {
                _callDepth = _threadCallDepth;
                _stackTraces = new List<string>
                {
                    Environment.StackTrace
                };
                if (_threadStackTraces != null) _stackTraces.AddRange(_threadStackTraces);
            }
            var tenantProvider = new TenantConfigurationValueProvider();
            _clientTenant = tenantProvider.Tenant;
            _callingClientName = tenantProvider.CallingClientName;
            _configuration = tenantProvider.LeverConfiguration;
            var correlationProvider = new CorrelationIdValueProvider();
            _correlationId = correlationProvider.CorrelationId;
        }

        /// <summary></summary>
        public void ExecuteActionFailSafe(Action<CancellationToken> action, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                RestoreContext();
                action(cancellationToken);
                lock (ClassLock)
                {
                    if (_threadStackTraces.Count != 0) _threadStackTraces.RemoveAt(_threadStackTraces.Count - 1);
                    _threadCallDepth--;
                }
            }
            catch (Exception e)
            {
                try
                {
                    var message = $"Background thread failed:\r{e.ToLogString()}." +
                                  $"\rApplication information: {FulcrumApplication.ToLogString()}" +
                                  $"\rContext information: {ToLogString()}";
                    Log.RecommendedForNetFramework.Log(LogSeverityLevel.Critical, message);
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
            lock (ClassLock)
            {
                _threadStackTraces = _stackTraces;
                _threadCallDepth = _callDepth + 1;
            }
            FulcrumAssert.IsLessThan(MaxDepthForBackgroundThreads, _threadCallDepth, null, "Too deep nesting of background jobs.");
        }

        /// <inheritdoc />
        public string ToLogString()
        {
            var message = $"Client: {_callingClientName} {_clientTenant} CorrelationId: {_correlationId}";
            lock (ClassLock)
            {
                if (_threadStackTraces == null) return message;
                message = _threadStackTraces
                    .Aggregate(message, (current, stackTrace) => current + $"\r\r{stackTrace}");
            }
            return message;
        }
    }
}
