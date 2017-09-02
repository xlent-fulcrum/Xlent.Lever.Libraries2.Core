namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// The severity level for a log message.
    /// </summary>
    public enum LogSeverityLevel
    {
        /// <summary>
        /// No severity level set. Should be treated as <see cref="Verbose"/> level.
        /// </summary>
        None = 0,
        /// <summary>
        /// Verbose level. Typically debug messages.
        /// </summary>
        Verbose = 1,
        /// <summary>
        /// Information level. Typically informational message of higher importance than <see cref="Verbose"/>.
        /// </summary>
        Information = 2,
        /// <summary>
        /// Warning level. The request could be handled, but the message describes an anomaly.
        /// </summary>
        Warning = 3,
        /// <summary>
        /// Error level. The request could not be fulfilled. Unique for this request.
        /// </summary>
        Error = 4,
        /// <summary>
        /// Critical error level. Something is wrong internally and all or many requests will fail.
        /// </summary>
        Critical = 5
    }
}