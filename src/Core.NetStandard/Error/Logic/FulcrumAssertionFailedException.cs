using System;

namespace Xlent.Lever.Libraries2.Core.Error.Logic
{
    /// <summary>
    /// The server failed to execute the request due to an assertion made by the programmer that proved to be wrong.
    /// </summary>
    /// <example>
    /// The programmer was sure that a certain condition would never be met, so the programmer just added an if-statement with this exception.
    /// </example>
    /// <remarks>
    /// This is basically a "Programmers Error", a bug on the server side.
    /// </remarks>
    public class FulcrumAssertionFailedException : FulcrumException
    {
        /// <summary>
        /// Factory method
        /// </summary>
        public static FulcrumException Create(string message, Exception innerException = null)
        {
            return new FulcrumAssertionFailedException(message, innerException);
        }

        /// <summary>
        /// The type for this <see cref="FulcrumException"/>
        /// </summary>
        public const string ExceptionType = "Xlent.Fulcrum.AssertionFailed";

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumAssertionFailedException() : this(null, (Exception)null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumAssertionFailedException(string message) : this(message, (Exception)null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumAssertionFailedException(string message, string errorLocation) : this(message, (Exception)null)
        {
            ErrorLocation = errorLocation;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumAssertionFailedException(string message, Exception innerException) : base(message, innerException)
        {
            SetProperties();
        }

        /// <inheritdoc />
        public override bool IsRetryMeaningful => false;

        /// <inheritdoc />
        public override string Type => ExceptionType;

        /// <inheritdoc />
        public override string FriendlyMessage =>
            "An assertion made by the programmer proved to be wrong and the request couldn't be properly fulfilled."
            + "\rPlease report the following:"
            + $"\rCorrelationId: {CorrelationId}"
            + $"\rInstanceId: {InstanceId}"
            + $"\rErrorLocation: {ErrorLocation ?? StackTrace}";

        private void SetProperties()
        {
            MoreInfoUrl = $"http://lever.xlent-fulcrum.info/FulcrumExceptions#{Type}";
        }
    }
}
