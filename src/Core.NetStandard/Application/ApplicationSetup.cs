using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Logging.Model;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Threads;

namespace Xlent.Lever.Libraries2.Core.Application
{
    /// <summary>
    /// A class with settings that are expected from in an application that uses Fulcrum libraries.
    /// </summary>
    public class ApplicationSetup : IValidatable
    {
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
        public ITenant Tenant { get; set; }

        /// <summary>
        /// How to deal with background threads.
        /// </summary>
        public IThreadHandler ThreadHandler { get; set; }

        /// <summary>
        /// The logger to use for logging for the entire application.
        /// </summary>
        public IFulcrumLogger Logger { get; set; }

        /// <summary>
        /// The context value provider that will be used all over the application.
        /// </summary>
        public IValueProvider ContextValueProvider { get; set; }

        /// <inheritdoc />
        public void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotNullOrWhiteSpace(Name, nameof(Name), errorLocation);
            FulcrumValidate.AreNotEqual(RunTimeLevelEnum.None, RunTimeLevel, nameof(RunTimeLevel), errorLocation);
            FulcrumValidate.IsValidated(Tenant, propertyPath, nameof(Tenant), errorLocation);
            FulcrumValidate.IsNotNull(ThreadHandler, nameof(ThreadHandler), errorLocation);
            FulcrumValidate.IsNotNull(Logger, nameof(Logger), errorLocation);
            FulcrumValidate.IsNotNull(ContextValueProvider, nameof(ContextValueProvider), errorLocation);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Tenant} {Name} ({RunTimeLevel})";
        }
    }
}