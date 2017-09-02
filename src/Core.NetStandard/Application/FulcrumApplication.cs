using System;
using System.Configuration;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Threads;

namespace Xlent.Lever.Libraries2.Core.Application
{
    /// <summary>
    /// Help class to setup your application
    /// </summary>
    public static class FulcrumApplication
    {
        /// <summary>
        /// The setup for the fulcrum application.
        /// </summary>
        public static readonly ApplicationSetup Setup = new ApplicationSetup();

        /// <summary>
        /// Use this to get application settings.
        /// </summary>
        public static AppSettings AppSettings { get; set; }

        /// <summary>
        /// Initialize <see cref="Setup"/>.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <param name="tenant">The tenant that the application itself runs in.</param>
        /// <param name="level">The run time level for the application itself.</param>
        /// <remarks>Will setup all mandatory fields for <see cref="Setup"/>, but you might want to override those values when this method returns."/></remarks>
        public static void Initialize(string name, ITenant tenant, RunTimeLevelEnum level)
        {
            InternalContract.RequireNotNullOrWhitespace(name, nameof(name));
            InternalContract.RequireValidated(tenant, nameof(tenant));
            InternalContract.Require(level != RunTimeLevelEnum.None, $"{nameof(level)} ({level}) must be set to something else than {RunTimeLevelEnum.None}");

            Setup.Name = name;
            Setup.Tenant = tenant;
            Setup.RunTimeLevel = level;

            Setup.ThreadHandler = ThreadHelper.RecommendedForNetFramework;
            Setup.Logger = Log.RecommendedForNetFramework;
            Setup.ContextValueProvider = ContextValueProvider.RecommendedForNetFramework;
        }

        /// <summary>
        /// Validate the values in <see cref="Setup"/> has been properly set.
        /// </summary>
        /// <remarks>This should be used initially for any Fulcrum library that depends on these settings.</remarks>
        public static void Validate()
        {
            InternalContract.RequireValidated(Setup, nameof(Setup), $"{typeof(FulcrumApplication).FullName} needs to be setup at application startup. Please use the {nameof(Initialize)} or a {nameof(FulcrumApplicationHelper)}.");
        }

        /// <summary>
        /// If we are in production, this method does nothing. Otherwise it calls <see cref="Validate"/>.
        /// </summary>
        public static void ValidateButNotInProduction()
        {
            if (Setup.RunTimeLevel == RunTimeLevelEnum.ProductionOrProductionLike) return;
            Validate();
        }
    }
}
