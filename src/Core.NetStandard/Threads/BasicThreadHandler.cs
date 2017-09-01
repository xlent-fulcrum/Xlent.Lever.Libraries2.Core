using System;
using System.Threading;

namespace Xlent.Lever.Libraries2.Core.Threads
{
    /// <summary>
    /// A basic thread handler based on well known concepts.
    /// </summary>
    public class BasicThreadHandler : IThreadHandler
    {
        /// <inheritdoc />
        public void FireAndForget(Action<CancellationToken> action)
        {
            var thread = new Thread(() => action(CancellationToken.None));
            thread.Start();
        }
    }
}