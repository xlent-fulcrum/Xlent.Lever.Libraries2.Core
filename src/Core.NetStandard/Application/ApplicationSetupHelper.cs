using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Logging.Logic;
using Xlent.Lever.Libraries2.Core.Threads;

namespace Xlent.Lever.Libraries2.Core.Application
{
    /// <summary>
    /// Help class to setup your application
    /// </summary>
    public static class ApplicationSetupHelper
    {
        /// <summary>
        /// Sets the recommended application setup for .NET Framework.
        /// </summary>
        public static void NetFrameworkSetup()
        {
            ApplicationSetup.ThreadHandler = ThreadHelper.RecommendedForNetFramework;
            ApplicationSetup.Logger = Log.RecommendedForNetFramework;
            ApplicationSetup.ContextValueProvider = ContextValueProvider.RecommendedForNetFramework;
        }
        /// <summary>
        /// Sets the recommended application setup for .NET Framework.
        /// </summary>
        public static void UnitTestSetup()
        {
            ApplicationSetup.ThreadHandler = ThreadHelper.RecommendedForNetFramework;
            ApplicationSetup.Logger = Log.RecommendedForNetFramework;
            ApplicationSetup.ContextValueProvider = ContextValueProvider.RecommendedForUnitTests;
        }
    }
}
