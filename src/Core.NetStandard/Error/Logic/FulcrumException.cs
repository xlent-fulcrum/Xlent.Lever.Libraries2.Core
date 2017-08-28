using System;
using System.Diagnostics;
using System.Linq;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Model;
using Xlent.Lever.Libraries2.Core.Misc;

namespace Xlent.Lever.Libraries2.Core.Error.Logic
{
    /// <summary>
    /// The base class for all Fulcrum exceptions
    /// </summary>
    public abstract class FulcrumException : Exception, IFulcrumError
    {
        /// <summary>
        /// The current servent name. Can be set by calling <see cref="Initialize"/>.
        /// Will automaticall be copied to the the field <see cref="ServerTechnicalName"/> for every new error.
        /// </summary>
        private static string _serverTechnicalName;

        /// <summary>
        /// Mandatory technical information that a developer might find useful.
        /// This is where you might include exception messages, stack traces, or anything else that you
        /// think will help a developer.
        /// </summary>
        /// <remarks>
        /// This message is not expected to contain any of the codes or identifiers that are already contained
        /// in this error type, sucha as the error <see cref="Code"/> or the <see cref="InstanceId"/>.
        /// </remarks>
        /// <remarks>
        /// If this property has not been set, the recommendation is to treat the <see cref="System.Exception.Message"/>
        /// property as the technical message.
        /// </remarks>
        public string TechnicalMessage { get; set; }

        /// <inheritdoc />
        public virtual string FriendlyMessage { get; set; }

        /// <inheritdoc />
        public string MoreInfoUrl { get; set; }

        /// <inheritdoc />
        public virtual bool IsRetryMeaningful { get; internal set; }

        /// <inheritdoc />
        public double RecommendedWaitTimeInSeconds { get; set; }

        /// <inheritdoc />
        public string ServerTechnicalName { get; set; }

        /// <inheritdoc />
        public string InstanceId { get; private set; }

        /// <inheritdoc />
        public string ErrorLocation { get; set; }

        /// <inheritdoc />
        public string Code { get; set; }

        /// <inheritdoc />
        public virtual string Type { get; private set; }

        /// <inheritdoc />
        public string CorrelationId { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        protected FulcrumException() : this(null, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        protected FulcrumException(string message) : this(message, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        protected FulcrumException(string message, Exception innerException) : base(message, innerException)
        {
            TechnicalMessage = message;
            var error = innerException as FulcrumException;
            if (error == null)
            {
                InstanceId = Guid.NewGuid().ToString();
                return;
            }
            FriendlyMessage = error.FriendlyMessage;
            MoreInfoUrl = error.MoreInfoUrl;
            RecommendedWaitTimeInSeconds = error.RecommendedWaitTimeInSeconds;
            InstanceId = error.InstanceId;
            CorrelationId = error.CorrelationId;
            ServerTechnicalName = _serverTechnicalName;
        }

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
            Type = fulcrumError.Type;
            CorrelationId = fulcrumError.CorrelationId;
            return this;
        }

        /// <summary>
        /// Sets the server technical name. This name will be used as a default for all new FulcrumExceptions.
        /// </summary>
        /// <param name="serverTechnicalName"></param>
        public static void Initialize(string serverTechnicalName)
        {
            InternalContract.RequireNotNullOrWhitespace(serverTechnicalName, nameof(serverTechnicalName));
            serverTechnicalName = serverTechnicalName.ToLower();
            if (_serverTechnicalName != null) InternalContract.Require(serverTechnicalName == _serverTechnicalName, 
                $"Once the server name has been set ({_serverTechnicalName}, it can't be changed ({serverTechnicalName}).");
            _serverTechnicalName = serverTechnicalName;
        }

        /// <inheritdoc />
        public virtual void Validate(string errorLocation, string propertyPath = "")
        {
            FulcrumValidate.IsNotNullOrWhiteSpace(TechnicalMessage, nameof(TechnicalMessage), errorLocation);
            FulcrumValidate.IsNotNullOrWhiteSpace(Type, nameof(Type), errorLocation);
            FulcrumValidate.IsNotNullOrWhiteSpace(InstanceId, nameof(InstanceId), errorLocation);
        }

        /// <inheritdoc />
        public override string StackTrace
        {
            get
            {
                var strings = new StackTrace(this, true)
                    .GetFrames()?
                    .Where(frame => !frame.GetMethod().IsDefined(typeof(StackTraceHiddenAttribute), true))
                    .Select(frame => new StackTrace(frame).ToString())
                    .ToArray();
                return strings != null ? string.Concat(strings) : "";
            }
        }
    }
}
