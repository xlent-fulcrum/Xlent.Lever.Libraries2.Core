using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Queue.Model
{
    /// <summary>
    /// A generic interface for reading items to a queue.
    /// </summary>
    public interface IPeekableQueue<T> : IBaseQueue
    {
        /// <summary>
        /// Returns the front item from the queue without removing it from the queue, or null if no items are on the queue.
        /// </summary>
        Task<T> PeekNoBlockAsync();
    }
}