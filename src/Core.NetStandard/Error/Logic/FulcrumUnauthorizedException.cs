using System;

namespace Xlent.Lever.Libraries2.Core.Error.Logic
{
    /// <summary>
    /// Authorization was missing or not accepted.
    /// </summary>
    public class FulcrumUnauthorizedException : FulcrumException
    {
        /// <summary>
        /// Factory method
        /// </summary>
        public static FulcrumException Create(string message, Exception innerException = null)
        {
            return new FulcrumUnauthorizedException(message, innerException);
        }

        /// <summary>
        /// The type for this <see cref="FulcrumException"/>.
        /// </summary>
        public const string ExceptionType = "Xlent.Fulcrum.Unauthorized";

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumUnauthorizedException() : this(null, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumUnauthorizedException(string message) : this(message, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumUnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        /// <inheritdoc />
        public override bool IsRetryMeaningful => false;

        /// <inheritdoc />
        public override string Type => ExceptionType;

        private void SetProperties()
        {
            FriendlyMessage =
                "Authorization was missing or not accepted.";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#FulcrumUnauthorizedException";
        }
    }
}
