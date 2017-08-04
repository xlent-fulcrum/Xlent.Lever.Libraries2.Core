using System;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Queue.Model
{
    /// <summary>
    /// A generic interface for adding items to a queue.
    /// </summary>
    public interface IWritableQueue<in T> : IBaseQueue
    {
        /// <summary>
        /// Add a message to the queue.
        /// </summary>
        /// <param name="message">The message to add</param>
        /// <param name="timeSpanToWait">An optional time span to wait before the item can be taken from the queue.</param>
        Task AddMessageAsync(T message, TimeSpan? timeSpanToWait = null);
    }
}
