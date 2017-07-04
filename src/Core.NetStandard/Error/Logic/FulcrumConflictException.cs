using System;

namespace Xlent.Lever.Libraries2.Core.Error.Logic
{
    /// <summary>
    /// The request conflicted with the current state of the resource.
    /// </summary>
    /// <example>
    /// Someone else has edited the resource (The Update with ETag scenario).
    /// </example>
    /// <example>
    /// Someone else has already created the resource (The Create or Insert scenario with duplicates).
    /// </example>
    public class FulcrumConflictException : FulcrumException
    {
        /// <summary>
        /// Factory method
        /// </summary>
        public static FulcrumException Create(string message, Exception innerException = null)
        {
            return new FulcrumConflictException(message, innerException);
        }

        /// <summary>
        /// The type for this <see cref="FulcrumException"/>.
        /// </summary>
        public const string ExceptionType = "Xlent.Fulcrum.Conflict";

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumConflictException() : this(null, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumConflictException(string message) : this(message, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumConflictException(string message, Exception innerException) : base(message, innerException)
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
                "The request conflicted with a request made by someone else. Please reload your data and make a new request.";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#FulcrumConflictException";
        }
    }
}
