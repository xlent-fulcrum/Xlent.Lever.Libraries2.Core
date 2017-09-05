﻿using System;
using System.Collections.Generic;
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
    internal class ContextPreservation : ILoggable
    {
        private const int MaxDepthForBackgroundThreads = 5;
        private readonly ITenant _clientTenant;
        private readonly string _correlationId;
        private readonly string _callingClientName;
        private readonly ILeverConfiguration _configuration;
        private readonly string _stackTrace;
        private readonly int _callDepth;

        [ThreadStatic]
        private static int _threadCallDepth;
        [ThreadStatic]
        private static Stack<string> _threadStackTraces;

        private static readonly object ClassLock = new object();

        public ContextPreservation()
        {
            lock (ClassLock)
            {
                if (_threadStackTraces == null) _threadStackTraces = new Stack<string>();
            }
            _callDepth = _threadCallDepth;
            _stackTrace = Environment.StackTrace;
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
                    Log.RecommendedForNetFramework.Log(LogSeverityLevel.Critical,
                        $"Background thread failed:\r{Log.FormatMessageFailSafe(e)}." +
                        $"\rApplication information: {FulcrumApplication.ToLogString()}" +
                        $"\rContext information: {ToLogString()}");
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
                _threadStackTraces.Push(_stackTrace);
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
                while (_threadStackTraces.Peek() != null)
                {
                    var stackTrace = _threadStackTraces.Pop();
                    message += $"\r{stackTrace}";
                }
            }
            return message;
        }
    }
}