namespace Xlent.Lever.Libraries2.Core.Context
{
    /// <summary>
    /// Interface for accessing a CorrelationId.
    /// </summary>
    public interface ICorrelationIdValueProvider
    {
        /// <summary>
        /// The value provider that is used to getting and setting data
        /// </summary>
        IValueProvider ValueProvider { get; }

        /// <summary>
        /// Access method for CorrelationId
        /// </summary>
        string CorrelationId { get; set; }
    }
}