using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly Tenant _clientTenant;
        private readonly string _correlationId;
        private readonly string _callingClientName;
        private readonly ILeverConfiguration _configuration;
        private readonly List<string> _stackTraces;
        private readonly int _callDepth;

        private static readonly AsyncLocal<int> ThreadCallDepth = new AsyncLocal<int> {Value = 0};
        private static readonly AsyncLocal<List<string>> ThreadStackTraces = new AsyncLocal<List<string>> {Value = null};

        private static readonly object ClassLock = new object();

        /// <summary></summary>
        public ContextPreservation()
        {
            lock (ClassLock)
            {
                _callDepth = ThreadCallDepth.Value;
                _stackTraces = new List<string>
                {
                    Environment.StackTrace
                };
                if (ThreadStackTraces.Value != null) _stackTraces.AddRange(ThreadStackTraces.Value);
            }
            var tenantProvider = new TenantConfigurationValueProvider();
            _clientTenant = tenantProvider.Tenant;
            _callingClientName = tenantProvider.CallingClientName;
            _configuration = tenantProvider.LeverConfiguration;
            var correlationProvider = new CorrelationIdValueProvider();
            _correlationId = correlationProvider.CorrelationId;
        }

        /// <summary>
        /// Restore the context, execute the action. Never throws an exception.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public void ExecuteActionFailSafe(Action<CancellationToken> action, CancellationToken token = default(CancellationToken))
        {
            try
            {
                RestoreContext();
                action(token);
                lock (ClassLock)
                {
                    if (ThreadStackTraces.Value.Count != 0) ThreadStackTraces.Value.RemoveAt(ThreadStackTraces.Value.Count - 1);
                    ThreadCallDepth.Value--;
                }
            }
            catch (Exception e)
            {
                SafeLog(e);
            }
        }

        /// <summary>
        /// Restore the context, execute the action. Never throws an exception.
        /// </summary>
        /// <param name="asyncMethod">The action to run in the background.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public async Task ExecuteActionFailSafeAsync(Func<CancellationToken, Task> asyncMethod, CancellationToken token = default(CancellationToken))
        {
            try
            {
                RestoreContext();
                await asyncMethod(token);
                lock (ClassLock)
                {
                    if (ThreadStackTraces.Value.Count != 0) ThreadStackTraces.Value.RemoveAt(ThreadStackTraces.Value.Count - 1);
                    ThreadCallDepth.Value--;
                }
            }
            catch (Exception e)
            {
                SafeLog(e);
            }
        }

        private void SafeLog(Exception e)
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
                ThreadStackTraces.Value = _stackTraces;
                ThreadCallDepth.Value = _callDepth + 1;
            }
            FulcrumAssert.IsLessThan(MaxDepthForBackgroundThreads, ThreadCallDepth.Value, null, "Too deep nesting of background jobs.");
        }

        /// <inheritdoc />
        public string ToLogString()
        {
            var message = $"Client: {_callingClientName} {_clientTenant} CorrelationId: {_correlationId}";
            lock (ClassLock)
            {
                if (ThreadStackTraces.Value == null) return message;
                message = ThreadStackTraces.Value
                    .Aggregate(message, (current, stackTrace) => current + $"\r\r{stackTrace}");
            }
            return message;
        }
    }
}
