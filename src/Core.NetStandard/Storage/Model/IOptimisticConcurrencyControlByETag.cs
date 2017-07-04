using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// This interface means that you implement ETag to achieve optimistic concurrency control, https://en.wikipedia.org/wiki/Optimistic_concurrency_control
    /// </summary>
    public interface IOptimisticConcurrencyControlByETag : IValidatable
    {
        /// <summary>
        /// ETag is a pattern to achieve optimistic concurrency control 
        /// </summary>
        /// <remarks>See https://en.wikipedia.org/wiki/HTTP_ETag for how it is used in the HTTP protocol. It can also be used for storage.</remarks>
        string ETag { get; set; }
    }
}
