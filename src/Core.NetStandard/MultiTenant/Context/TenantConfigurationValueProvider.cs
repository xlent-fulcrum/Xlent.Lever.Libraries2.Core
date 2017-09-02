using System;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Platform.Configurations;

namespace Xlent.Lever.Libraries2.Core.MultiTenant.Context
{
    /// <summary>
    /// Stores values in the execution context which is unaffected by asynchronous code that switches threads or context. 
    /// </summary>
    /// <remarks>Updating values in a thread will not affect the value in parent/sibling threads</remarks>
    public class TenantConfigurationValueProvider : ITenantConfigurationValueProvider
    {
        private const string TenantIdKey = "TenantId";
        private const string LeverConfigurationIdKey = "LeverConfigurationId";
        private const string CallingClientNameKey = "CallingClientName";

        /// <summary>
        /// An instances based on <see cref="AsyncLocalValueProvider"/>.
        /// </summary>
        [Obsolete("Create your own instance", true)]
        public static ITenantConfigurationValueProvider AsyncLocalInstance { get; } = new TenantConfigurationValueProvider(new AsyncLocalValueProvider());

        /// <summary>
        /// An instances based on <see cref="SingleThreadValueProvider"/>.
        /// </summary>
        [Obsolete("Create your own instance", true)]
        public static ITenantConfigurationValueProvider MemoryCacheInstance { get; } = new TenantConfigurationValueProvider(new SingleThreadValueProvider());

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="valueProvider">The value provider to use for getting and setting.</param>
        [Obsolete("Use the empty constructor.")]
        public TenantConfigurationValueProvider(IValueProvider valueProvider) : this()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TenantConfigurationValueProvider()
        {
            FulcrumApplication.Validate();
            ValueProvider = FulcrumApplication.Setup.ContextValueProvider;
        }

        /// <inheritdoc />
        public IValueProvider ValueProvider { get; }

        /// <inheritdoc />
        public ITenant Tenant
        {
            get => ValueProvider.GetValue<Tenant>(TenantIdKey);
            set => ValueProvider.SetValue(TenantIdKey, value);
        }

        /// <inheritdoc />
        public ILeverConfiguration LeverConfiguration
        {
            get => ValueProvider.GetValue<ILeverConfiguration>(LeverConfigurationIdKey);
            set => ValueProvider.SetValue(LeverConfigurationIdKey, value);
        }

        /// <inheritdoc />
        public string CallingClientName
        {
            get => ValueProvider.GetValue<string>(CallingClientNameKey);
            set => ValueProvider.SetValue(CallingClientNameKey, value);
        }
    }
}
