﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Application;
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
            FulcrumApplication.Setup.ThreadHandler.FireAndForget(t1 => context.ExecuteActionFailSafe(t2 => action(), CancellationToken.None));
        }

        /// <summary>
        /// Execute an <paramref name="action"/> in the background.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public static void FireAndForget(Action<CancellationToken> action, CancellationToken token = default(CancellationToken))
        {
            FulcrumApplication.ValidateButNotInProduction();
            var context = new ContextPreservation();
            FulcrumApplication.Setup.ThreadHandler.FireAndForget(t => context.ExecuteActionFailSafe(action, t), token);
        }

        /// <summary>
        /// Execute an <paramref name="asyncMethod"/> in the background.
        /// </summary>
        /// <param name="asyncMethod">The action to run in the background.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public static void FireAndForget(Func<CancellationToken, Task> asyncMethod, CancellationToken token = default(CancellationToken))
        {
            FulcrumApplication.ValidateButNotInProduction();
            var context = new ContextPreservation();
            FulcrumApplication.Setup.ThreadHandler.FireAndForget(async t => await context.ExecuteActionFailSafeAsync(asyncMethod, t), token);
        }

        /// <summary>
        /// Execute an <paramref name="asyncMethod"/> in the background.
        /// </summary>
        /// <param name="asyncMethod">The action to run in the background.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public static void CallAsyncFromSync(Func<CancellationToken, Task> asyncMethod, CancellationToken token = default(CancellationToken))
        {
            // This way to call an async method from a synchronous method was found here:
            // https://stackoverflow.com/questions/40324300/calling-async-methods-from-non-async-code
            Task.Run(async () => await asyncMethod(token), token).Wait(token);
        }

        /// <summary>
        /// Execute an <paramref name="asyncMethod"/> in the background.
        /// </summary>
        /// <param name="asyncMethod">The action to run in the background.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public static void CallAsyncFromSync(Func<Task> asyncMethod, CancellationToken token = default(CancellationToken))
        {
            // This way to call an async method from a synchronous method was found here:
            // https://stackoverflow.com/questions/40324300/calling-async-methods-from-non-async-code
            Task.Run(asyncMethod, token).Wait(token);
        }

        /// <summary>
        /// Default <see cref="IValueProvider"/> for .NET Framework.
        /// </summary>
        public static IThreadHandler RecommendedForNetFramework { get; } = new BasicThreadHandler();

    }
}
