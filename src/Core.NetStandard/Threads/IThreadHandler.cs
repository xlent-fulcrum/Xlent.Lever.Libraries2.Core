using System;
using System.Threading;

namespace Xlent.Lever.Libraries2.Core.Threads
{
    /// <summary>
    /// Required functionality for a thread handler
    /// </summary>
    public interface IThreadHandler
    {
        /// <summary>
        /// Execute an <paramref name="action"/> in the background.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        void FireAndForget(Action<CancellationToken> action);
    }
}