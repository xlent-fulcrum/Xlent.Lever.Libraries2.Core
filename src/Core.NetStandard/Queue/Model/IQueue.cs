using Xlent.Lever.Libraries2.Core.Health.Model;

namespace Xlent.Lever.Libraries2.Core.Queue.Model
{
    /// <summary>
    /// A generic interface for adding strings to a queue.
    /// </summary>
    public interface IBaseQueue : IResourceHealth
    {
        /// <summary>
        /// The name of the queue.
        /// </summary>
        string Name { get; }
    }
}
