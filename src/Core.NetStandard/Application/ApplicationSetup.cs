using System;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Threads;

namespace Xlent.Lever.Libraries2.Core.Application
{
    /// <summary>
    /// A class with settings that are expected from in an application that uses Fulcrum libraries.
    /// </summary>
    public class ApplicationSetup : IValidatable
    {
#pragma warning disable 618
        private IFulcrumLogger _logger;
#pragma warning restore 618

        /// <summary>
        /// The name of the application.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current <see cref="RunTimeLevel"/> of the application. Affects logging, testing, etc.
        /// </summary>
        /// <remarks>For a multi tenant application, this is the run time level for the application itself, not it's tenants.</remarks>
        public RunTimeLevelEnum RunTimeLevel { get; set; }

        /// <summary>
        /// The tenant for the application. For a multi-tenant application, this is the application tenant not any caller tenant.
        /// </summary>
        public Tenant Tenant { get; set; }

        /// <summary>
        /// How to deal with background threads.
        /// </summary>
        public IThreadHandler ThreadHandler { get; set; }

        /// <summary>
        /// The logger to use for logging for the entire application.
        /// </summary>
        [Obsolete("Use FullLogger")]
        public IFulcrumLogger Logger
        {
            get => _logger ?? FullLogger;
            set
            {
                _logger = value;
                if (value is IFulcrumFullLogger fullLogger) FullLogger = fullLogger;
            }
        }

        /// <summary>
        /// The logger to use for logging for the entire application.
        /// </summary>
        public IFulcrumFullLogger FullLogger { get; set; }

        /// <summary>
        /// The context value provider that will be used all over the application.
        /// </summary>
        public IValueProvider ContextValueProvider { get; set; }

        /// <summary>
        /// If any log in a batch of logs has a severitylevel that is equal to or higher than this level,
        /// then <see cref="LogSeverityLevelThreshold"/> is overriden and all logs will be used.
        /// </summary>
        public LogSeverityLevel BatchLogAllSeverityLevelThreshold { get; set; }

        /// <summary>
        /// A log must have at least this level to be sent for logging. Can be overridden in batches by <see cref="BatchLogAllSeverityLevelThreshold"/>.
        /// </summary>
        public LogSeverityLevel LogSeverityLevelThreshold{ get; set; }

        /// <inheritdoc />
        public void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotNullOrWhiteSpace(Name, nameof(Name), errorLocation);
            FulcrumValidate.AreNotEqual(RunTimeLevelEnum.None, RunTimeLevel, nameof(RunTimeLevel), errorLocation);
            FulcrumValidate.IsValidated(Tenant, propertyPath, nameof(Tenant), errorLocation);
            FulcrumValidate.IsNotNull(ThreadHandler, nameof(ThreadHandler), errorLocation);
#pragma warning disable 618
            if (FullLogger == null) FulcrumValidate.IsNotNull(Logger, nameof(Logger), errorLocation);
#pragma warning restore 618
            FulcrumValidate.IsNotNull(ContextValueProvider, nameof(ContextValueProvider), errorLocation);
            FulcrumValidate.IsGreaterThanOrEqualTo((int)LogSeverityLevel.Verbose, (int)LogSeverityLevelThreshold, nameof(LogSeverityLevelThreshold), errorLocation);
            FulcrumValidate.IsGreaterThanOrEqualTo((int)LogSeverityLevel.Verbose, (int)LogSeverityLevelThreshold, nameof(LogSeverityLevelThreshold), errorLocation);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name} {Tenant} ({RunTimeLevel})";
        }
    }
}