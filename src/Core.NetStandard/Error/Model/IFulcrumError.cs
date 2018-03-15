using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Error.Model
{
    /// <summary>
    /// An interface for error information. Typically used by <see cref="FulcrumException"/>.
    /// </summary>
    public interface IFulcrumError : IValidatable
    {
        /// <summary>
        /// Mandatory technical information that a developer might find useful.
        /// This is where you might include exception messages, stack traces, or anything else that you
        /// think will help a developer.
        /// </summary>
        /// <remarks>
        /// This message is not expected to contain any of the codes or identifiers that are already contained
        /// in this error type, sucha as the error <see cref="Code"/> or the <see cref="InstanceId"/>.
        /// </remarks>
        string TechnicalMessage { get; }

        /// <summary>
        /// An optional human readable error message that can potentially be shown directly to an application
        /// end user (not a developer). It should be friendly and easy to understand and convey a concise
        /// reason as to why the error occurred.  It should probaby not contain technical information. 
        /// </summary>
        string FriendlyMessage { get; }

        /// <summary>
        /// Errors are grouped into different types, such as "BusinessRule", "NotFound", "Unavailable".
        /// Type is a mandatory unique id for the type of error. The recommendation is to use a readable string
        /// such as "Xlent.Fulcrum.AssertionFailed"
        /// </summary>
        string Type { get; }

        /// <summary>
        /// An optional URL that anyone seeing the error message can click (or copy and paste) in a browser.
        /// The target web page should describe the error condition fully, as well as potential solutions
        /// to help them resolve the error condition.
        /// </summary>
        string MoreInfoUrl { get; }

        /// <summary>
        /// Mandatory indication for if it would be meaningful to try sending the request again.
        /// </summary>
        bool IsRetryMeaningful { get; }

        /// <summary>
        /// If <see cref="IsRetryMeaningful"/> is true, then this optional property can give a recommended
        /// interval to wait before the request is sent again. A value less or equal to 0.0 means that
        /// no recommendation was given.
        /// </summary>
        double RecommendedWaitTimeInSeconds { get; }

        /// <summary>
        /// An optional technical name for the server that created this error information.
        /// </summary>
        /// <remarks>
        /// Useful when for a call that is "deep", i.e. the call was relayed to another server.
        /// </remarks>
        string ServerTechnicalName { get; }

        /// <summary>
        /// A mandatory unique identifier for this particular instance of the error. Ideally, the same identifier
        /// should not be used ever again. The recommendation is to use a newly created GUID.
        /// </summary>
        string InstanceId { get; }

        /// <summary>
        /// An optional hint on where this error occurred in the code. The recommendation is to use the name of the DLL file combined with a fixed GUID for the specific location within the DLL.
        /// </summary>
        string ErrorLocation { get; }

        /// <summary>
        /// An optional error code for the error. A way to use a standard <see cref="Type"/>, but still be more specific. Will typically  be a part of the <see cref="MoreInfoUrl"/>.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// All calls that were involved in the chain that led to this error (successful calls or not) will
        /// all be marked in the logs with this mandatory CorrelationId. It is valuable if someone wants to track down
        /// exactly what happened.
        /// </summary>
        string CorrelationId { get; }

        /// <summary>
        /// Something like an inner exception; if this fulcrum error happens when dealing with another error, this is that error.
        /// </summary>
        FulcrumError InnerError { get; set; }

        /// <summary>
        /// Copies all fields from <paramref name="fulcrumError"/>.
        /// </summary>
        IFulcrumError CopyFrom(IFulcrumError fulcrumError);
    }
}