using System;

namespace Xlent.Lever.Libraries2.Core.Error.Logic
{
    /// <summary>
    /// The request requires some functionality that deliberately has been left out, at least for the moment.
    /// </summary>
    /// <example>
    /// During testing, there could be parts of the code that hasn't been developed yet. Then throwing this exception is appropriate.
    /// </example>
    public class FulcrumNotImplementedException : FulcrumException
    {
        /// <summary>
        /// Factory method
        /// </summary>
        public static FulcrumException Create(string message, Exception innerException = null)
        {
            return new FulcrumNotImplementedException(message, innerException);
        }

        /// <summary>
        /// The type for this <see cref="FulcrumException"/>.
        /// </summary>
        public const string ExceptionType = "Xlent.Fulcrum.NotImplemented";

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumNotImplementedException() : this(null, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumNotImplementedException(string message) : this(message, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumNotImplementedException(string message, Exception innerException) : base(message, innerException)
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
                "The request requires some functionality that deliberately has been left out, at least for the moment.";
            FriendlyMessage += "Please report the following:";
            FriendlyMessage += $"\rCorrelactionId: {CorrelationId}";
            FriendlyMessage += $"\rInstanceId: {InstanceId}";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#FulcrumNotImplementedException";
        }
    }
}
