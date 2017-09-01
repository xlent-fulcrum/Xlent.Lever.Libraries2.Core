using System;
using System.Threading;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;

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
        /// <remarks>There are overrides for this, see e.g. in Xlent.Lever.Libraries2.WebApi.Context.</remarks>
        protected static IThreadHandler ChosenThreadHandler;

        /// <summary>
        /// The chosen <see cref="IValueProvider"/> to use.
        /// </summary>
        /// <remarks>There are overrides for this, see e.g. in Xlent.Lever.Libraries2.WebApi.Context.</remarks>
        public static IThreadHandler ThreadHandlerForApplication
        {
            get
            {
                // TODO: Link to Lever WIKI
                FulcrumAssert.IsNotNull(ChosenThreadHandler, null, $"The application must at startup set {nameof(ThreadHandlerForApplication)} to the appropriate {nameof(IThreadHandler)}.");
                return ChosenThreadHandler;
            }
            set
            {
                InternalContract.RequireNotNull(value, nameof(value));
                ChosenThreadHandler = value;
            }
        }
        /// <summary>
        /// Execute an <paramref name="action"/> in the background.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        public static void FireAndForget(Action action)
        {
            FireAndForget(cancellationToken => action());
        }

        /// <summary>
        /// Execute an <paramref name="action"/> in the background.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        public static void FireAndForget(Action<CancellationToken> action)
        {
            ThreadHandlerForApplication.FireAndForget(action);
        }

        /// <summary>
        /// Default <see cref="IValueProvider"/> for .NET Framework.
        /// </summary>
        public static IThreadHandler RecommendedForNetFramework { get; } = new BasicThreadHandler();

    }
}
