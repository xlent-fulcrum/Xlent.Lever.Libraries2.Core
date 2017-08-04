using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Queue.Model
{
    /// <summary>
    /// A generic interface for reading items to a queue.
    /// </summary>
    public interface IReadableQueue<T>
    {
        /// <summary>
        /// Remove all items from the queue
        /// </summary>
        Task ClearAsync();

        /// <summary>
        /// Returns one item from the queue, or null if no items are on the queue.
        /// </summary>
        Task<T> GetOneMessageNoBlockAsync();
    }
}