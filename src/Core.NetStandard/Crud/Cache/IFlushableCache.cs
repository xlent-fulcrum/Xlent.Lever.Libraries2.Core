using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Cache
{
    /// <summary>
    /// Interface for a flush method for the cache. The flush method should reset the cache to empty.
    /// </summary>
    public interface IFlushableCache
    {
        /// <summary>
        /// Clears the cache, i.e. remove all cached items.
        /// </summary>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        Task FlushAsync(CancellationToken token = default(CancellationToken));
    }
}
