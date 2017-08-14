using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Error.Model
{
    /// <summary>
    /// Information that will be returned when a REST service returns a non successful HTTP status code
    /// </summary>
    /// <remarks>
    /// Inspired by the follwing articles
    /// http://blog.restcase.com/rest-api-error-codes-101/
    /// https://stormpath.com/blog/spring-mvc-rest-exception-handling-best-practices-part-1
    /// </remarks>
    public class FulcrumError : IFulcrumError
    {
        /// <inheritdoc />
        public string TechnicalMessage { get; set; }

        /// <inheritdoc />
        public string FriendlyMessage { get; set; }

        /// <inheritdoc />
        public string MoreInfoUrl { get; set; }

        /// <inheritdoc />
        public bool IsRetryMeaningful { get; set; }

        /// <inheritdoc />
        public double RecommendedWaitTimeInSeconds { get; set; }

        /// <inheritdoc />
        public string ServerTechnicalName { get; set; }

        /// <inheritdoc />
        public string InstanceId { get; set; }

        /// <inheritdoc />
        public string ErrorLocation { get; set; }

        /// <inheritdoc />
        public string Code { get; set; }

        /// <inheritdoc />
        public string Type { get; set; }

        /// <inheritdoc />
        public string CorrelationId { get; set; }

        /// <inheritdoc />
        public string FriendlyMessageId { get; set; }

        /// <inheritdoc />
        public FulcrumError InnerError { get; set; }

        /// <inheritdoc />
        public IFulcrumError CopyFrom(IFulcrumError fulcrumError)
        {
            TechnicalMessage = fulcrumError.TechnicalMessage;
            FriendlyMessage = fulcrumError.FriendlyMessage;
            MoreInfoUrl = fulcrumError.MoreInfoUrl;
            IsRetryMeaningful = fulcrumError.IsRetryMeaningful;
            RecommendedWaitTimeInSeconds = fulcrumError.RecommendedWaitTimeInSeconds;
            ServerTechnicalName = fulcrumError.ServerTechnicalName;
            InstanceId = fulcrumError.InstanceId;
            Code = fulcrumError.Code;
            ErrorLocation = fulcrumError.ErrorLocation;
            Type = fulcrumError.Type;
            CorrelationId = fulcrumError.CorrelationId;
            return this;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return TechnicalMessage;
        }

        /// <inheritdoc />
        public virtual void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotNullOrWhiteSpace(TechnicalMessage, nameof(TechnicalMessage), errorLocation);
            FulcrumValidate.IsNotNullOrWhiteSpace(Type, nameof(Type), errorLocation);
            FulcrumValidate.IsNotNullOrWhiteSpace(InstanceId, nameof(InstanceId), errorLocation);
        }
    }
}
