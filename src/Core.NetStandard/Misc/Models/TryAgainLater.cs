using System;

namespace Xlent.Lever.Libraries2.Core.Misc.Models
{
    /// <summary>
    /// Information to be provided when a request has been accepted, but the response is not yet ready.
    /// </summary>
    public class TryAgainLater
    {
        /// <summary>
        /// The URL where a GET will fetch the response once it is ready.
        /// </summary>
        public string LocationUrl { get; set; }

        /// <summary>
        /// The estimated time that the reponse should be available at <see cref="LocationUrl"/>.
        /// </summary>
        public DateTimeOffset EstimatedReadyAt { get; set; }
    }
}