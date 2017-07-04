using System;

namespace Xlent.Lever.Libraries2.Core.Error.Logic
{
    /// <summary>
    /// The specified item could not be found.
    /// </summary>
    /// <example>
    /// A request for a person with a specified Id that doesn't exist should always throw this exception.
    /// </example>
    /// <example>
    /// If a person exists, and the request is for a list of e-mail addresses and the person doesn't have any, 
    /// you should not throw this exception, but return an empty list.
    /// </example>
    public class FulcrumNotFoundException : FulcrumException
    {
        /// <summary>
        /// Factory method
        /// </summary>
        public static FulcrumException Create(string message, Exception innerException = null)
        {
            return new FulcrumNotFoundException(message, innerException);
        }

        /// <summary>
        /// The type for this <see cref="FulcrumException"/>.
        /// </summary>
        public const string ExceptionType = "Xlent.Fulcrum.NotFound";

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumNotFoundException() : this(null, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumNotFoundException(string message) : this(message, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        /// <inheritdoc />
        public override bool IsRetryMeaningful => true;

        /// <inheritdoc />
        public override string Type => ExceptionType;

        private void SetProperties()
        {
            if (RecommendedWaitTimeInSeconds <= 0.0) RecommendedWaitTimeInSeconds = 60;
            if (FriendlyMessage == null)
            {
                FriendlyMessage =
                    "The resource with the given identification could not be found. Check that your information is correct or try again after a minute or so.";
            }

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#FulcrumNotFoundException";
        }
    }
}
