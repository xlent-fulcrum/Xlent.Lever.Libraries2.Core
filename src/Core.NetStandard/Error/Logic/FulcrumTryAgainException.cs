using System;

namespace Xlent.Lever.Libraries2.Core.Error.Logic
{
    /// <summary>
    /// The resource was temporarily locked, please try again in the recommended time span (<see cref="FulcrumException.RecommendedWaitTimeInSeconds"/>).
    /// </summary>
    public class FulcrumTryAgainException : FulcrumException
    {
        /// <summary>
        /// Factory method
        /// </summary>
        public static FulcrumException Create(string message, Exception innerException = null)
        {
            return new FulcrumTryAgainException(message, innerException);
        }

        /// <summary>
        /// The type for this <see cref="FulcrumException"/>.
        /// </summary>
        public const string ExceptionType = "Xlent.Fulcrum.TryAgain";

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumTryAgainException() : this(null, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumTryAgainException(string message) : this(message, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumTryAgainException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        /// <inheritdoc />
        public override bool IsRetryMeaningful => true;

        /// <inheritdoc />
        public override string Type => ExceptionType;

        private void SetProperties()
        {
            if (RecommendedWaitTimeInSeconds <= 0.0) RecommendedWaitTimeInSeconds = 2;

            FriendlyMessage =
                "The resource was temporarily locked, please try again.";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#FulcrumTryAgainException";
        }
    }
}
