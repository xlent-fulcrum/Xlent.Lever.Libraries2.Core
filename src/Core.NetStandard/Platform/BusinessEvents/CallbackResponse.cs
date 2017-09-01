namespace Xlent.Lever.Libraries2.Core.Platform.BusinessEvents
{
    /// <summary>
    /// The payload that Business Events sends when it gets the response from an event it sent.
    /// </summary>
    /// <remarks>Turn this feature on in for your tenant in Configrations for BusinessEvents with the setting PublishAtCallback = true</remarks>
    public class CallbackResponse
    {
        /// <summary>
        /// The correlation id from the Correlation Id Provider, which will inspect the X-Correlation-ID header
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// The name of the original event that was sent, on the form Entity.Event/MajorVErsion.MinorVersion
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Http status code from the subscriber
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// The subscription url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The subscriber's client name (as defined in the BusinessEvents Service)
        /// </summary>
        public string SubscriberClientName { get; set; }
    }
}
