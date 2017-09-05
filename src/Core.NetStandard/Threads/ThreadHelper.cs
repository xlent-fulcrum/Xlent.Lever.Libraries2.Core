using System;
using System.Threading;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Logging;

namespace Xlent.Lever.Libraries2.Core.Threads
{
    /// <summary>
    /// Convenience for choosing the right <see cref="IThreadHandler"/>.
    /// </summary>
    public class ThreadHelper
    {
        /// <summary>
        /// The chosen <see cref="IThreadHandler"/> to use.
        /// </summary>
        /// <remarks>There are overrides for this, see e.g. in Xlent.Lever.Libraries2.WebApi.ContextValueProvider.</remarks>
        [Obsolete("Use FulcrumApplication.Setup.ThreadHandler", true)]
        protected static IThreadHandler ChosenThreadHandler;

        /// <summary>
        /// The chosen <see cref="IValueProvider"/> to use.
        /// </summary>
        /// <remarks>There are overrides for this, see e.g. in Xlent.Lever.Libraries2.WebApi.ContextValueProvider.</remarks>
        [Obsolete("Use FulcrumApplication.Setup.ThreadHandler", true)]
        public static IThreadHandler ThreadHandlerForApplication
        {
            get => FulcrumApplication.Setup.ThreadHandler;
            set => FulcrumApplication.Setup.ThreadHandler = value;
        }

        /// <summary>
        /// Execute an <paramref name="action"/> in the background.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        public static void FireAndForget(Action action)
        {
            FulcrumApplication.ValidateButNotInProduction();
            var context = new ContextPreservation();
            FulcrumApplication.Setup.ThreadHandler.FireAndForget(cancellationToken => context.ExecuteActionFailSafe(token => action(), CancellationToken.None));
        }

        /// <summary>
        /// Execute an <paramref name="action"/> in the background.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        public static void FireAndForget(Action<CancellationToken> action)
        {
            FulcrumApplication.ValidateButNotInProduction();
            var context = new ContextPreservation();
            FulcrumApplication.Setup.ThreadHandler.FireAndForget(cancellationToken => context.ExecuteActionFailSafe(action, cancellationToken));
        }
        

        /// <summary>
        /// Default <see cref="IValueProvider"/> for .NET Framework.
        /// </summary>
        public static IThreadHandler RecommendedForNetFramework { get; } = new BasicThreadHandler();

    }
}
