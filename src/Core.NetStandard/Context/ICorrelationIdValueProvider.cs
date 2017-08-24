namespace Xlent.Lever.Libraries2.Core.Context
{
    /// <summary>
    /// Interface for accessing a CorrelationId.
    /// </summary>
    public interface ICorrelationIdValueProvider : IHasValueProvider
    {

        /// <summary>
        /// Access method for CorrelationId
        /// </summary>
        string CorrelationId { get; set; }
    }
}