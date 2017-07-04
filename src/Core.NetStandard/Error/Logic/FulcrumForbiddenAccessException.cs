using System;

namespace Xlent.Lever.Libraries2.Core.Error.Logic
{
    /// <summary>
    /// Authorization existed, but access was forbidden.
    /// </summary>
    public class FulcrumForbiddenAccessException : FulcrumException
    {
        /// <summary>
        /// Factory method
        /// </summary>
        public static FulcrumException Create(string message, Exception innerException = null)
        {
            return new FulcrumForbiddenAccessException(message, innerException);
        }

        /// <summary>
        /// The type for this <see cref="FulcrumException"/>.
        /// </summary>
        public const string ExceptionType = "Xlent.Fulcrum.ForbiddenAccess";

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumForbiddenAccessException() : this(null, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumForbiddenAccessException(string message) : this(message, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumForbiddenAccessException(string message, Exception innerException) : base(message, innerException)
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
                "The system could identify you, but you did not have right to access the resource.";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#FulcrumForbiddenAccessException";
        }
    }
}
