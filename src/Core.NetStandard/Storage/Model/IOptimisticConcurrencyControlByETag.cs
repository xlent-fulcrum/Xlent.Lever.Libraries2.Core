namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// This interface means that you implement Etag to achieve optimistic concurrency control, https://en.wikipedia.org/wiki/Optimistic_concurrency_control
    /// </summary>
    public interface IOptimisticConcurrencyControlByETag
    {
        /// <summary>
        /// Etag is a pattern to achieve optimistic concurrency control 
        /// </summary>
        /// <remarks>See https://en.wikipedia.org/wiki/HTTP_ETag for how it is used in the HTTP protocol. It can also be used for storage.</remarks>
        string Etag { get; set; }
    }
}
