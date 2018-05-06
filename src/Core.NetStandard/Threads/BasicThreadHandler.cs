using System;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Threads
{
    /// <summary>
    /// A basic thread handler based on well known concepts.
    /// </summary>
    public class BasicThreadHandler : IThreadHandler
    {
        /// <inheritdoc />
        public void FireAndForget(Action<CancellationToken> action, CancellationToken token = default(CancellationToken))
        {
            var thread = new Thread(() => action(token));
            thread.Start();
        }
    }
}