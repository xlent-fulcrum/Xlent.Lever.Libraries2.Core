using System;

namespace Xlent.Lever.Libraries2.Core.Error.Logic
{
    /// <summary>
    /// The request conflicted with a business rule.
    /// </summary>
    public class FulcrumBusinessRuleException : FulcrumException
    {
        /// <summary>
        /// Factory method
        /// </summary>
        public static FulcrumException Create(string message, Exception innerException = null)
        {
            return new FulcrumBusinessRuleException(message, innerException);
        }

        /// <summary>
        /// The Type for this kind of <see cref="FulcrumException"/>.
        /// </summary>
        public const string ExceptionType = "Xlent.Fulcrum.BusinessRule";

        /// <summary>
        /// Empty constructor
        /// </summary>
        public FulcrumBusinessRuleException() : this(null, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumBusinessRuleException(string message) : this(message, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public FulcrumBusinessRuleException(string message, Exception innerException) : base(message, innerException)
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
                "The request conflicted with a business rules. Please make changes accordingly and try again.";

            MoreInfoUrl = "http://lever.xlent-fulcrum.info/FulcrumExceptions#FulcrumBusinessRuleException";
        }
    }
}
