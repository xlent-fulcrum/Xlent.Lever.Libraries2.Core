using System.Configuration;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Logging.Logic;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Threads;

namespace Xlent.Lever.Libraries2.Core.Application
{
    /// <summary>
    /// Help class to setup your application
    /// </summary>
    public static class FulcrumApplicationHelper
    {
        /// <summary>
        /// Sets the recommended application setup for .NET Framework.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <param name="tenant">The tenant that the application itself runs in.</param>
        /// <param name="runTimeLevel">The <see cref="RunTimeLevelEnum"/> for the application itself.</param>
        public static void NetFrameworkSetup(string name, ITenant tenant, RunTimeLevelEnum runTimeLevel)
        {
            FulcrumApplication.Initialize(name, tenant, runTimeLevel);
            FulcrumApplication.Setup.ThreadHandler = ThreadHelper.RecommendedForNetFramework;
            FulcrumApplication.Setup.Logger = Log.RecommendedForNetFramework;
            FulcrumApplication.Setup.ContextValueProvider = ContextValueProvider.RecommendedForNetFramework;
        }

        /// <summary>
        /// Sets the recommended application setup for .NET Framework.
        /// </summary>
        /// <paramref name="appSettingGetter"/>How to get app settings for <see cref="ApplicationSetup.Name"/>, <see cref="ApplicationSetup.Tenant"/>, <see cref="ApplicationSetup.RunTimeLevel"/>
        /// <remarks>If you want to use <see cref="ConfigurationManager"/> for retreiving app settings, you can use <see cref="ConfigurationManagerAppSettings"/> as the <paramref name="appSettingGetter"/>.</remarks>
        public static void NetFrameworkSetup(IAppSettingGetter appSettingGetter)
        {
            InternalContract.RequireNotNull(appSettingGetter, nameof(appSettingGetter));
            var appSettings = new AppSettings(appSettingGetter);
            var name = appSettings.GetString("ApplicationName", true);
            var tenant = appSettings.GetTenant("Organization", "Environment", true);
            var runTimeLevel = appSettings.GetEnum<RunTimeLevelEnum>("RunTimeLevel", true);
            NetFrameworkSetup(name, tenant, runTimeLevel);
        }

        /// <summary>
        /// Sets the recommended application setup for unit testing.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        public static void UnitTestSetup(string name)
        {
            FulcrumApplication.Initialize(name, new Tenant("unknown", "local"), RunTimeLevelEnum.Development);
            FulcrumApplication.Setup.ThreadHandler = ThreadHelper.RecommendedForNetFramework;
            FulcrumApplication.Setup.Logger = Log.RecommendedForNetFramework;
            FulcrumApplication.Setup.ContextValueProvider = ContextValueProvider.RecommendedForUnitTests;
        }

    }
}
