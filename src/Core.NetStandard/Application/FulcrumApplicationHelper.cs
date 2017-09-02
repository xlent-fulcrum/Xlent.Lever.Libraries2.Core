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
        /// <param name="level">The run time level for the application itself.</param>
        public static void NetFrameworkSetup(string name, ITenant tenant, RunTimeLevelEnum level)
        {
            FulcrumApplication.Initialize(name, tenant, level);
            FulcrumApplication.Setup.ThreadHandler = ThreadHelper.RecommendedForNetFramework;
            FulcrumApplication.Setup.Logger = Log.RecommendedForNetFramework;
            FulcrumApplication.Setup.ContextValueProvider = ContextValueProvider.RecommendedForNetFramework;
        }
        /// <summary>
        /// Sets the recommended application setup for .NET Framework.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <param name="tenant">The tenant that the application itself runs in.</param>
        /// <param name="level">The run time level for the application itself.</param>
        public static void UnitTestSetup(string name, ITenant tenant, RunTimeLevelEnum level)
        {
            FulcrumApplication.Initialize(name, tenant, level);
            FulcrumApplication.Setup.ThreadHandler = ThreadHelper.RecommendedForNetFramework;
            FulcrumApplication.Setup.Logger = Log.RecommendedForNetFramework;
            FulcrumApplication.Setup.ContextValueProvider = ContextValueProvider.RecommendedForUnitTests;
        }
    }
}
