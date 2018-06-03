using System;
using System.Runtime.CompilerServices;

namespace Xlent.Lever.Libraries2.Core.Logging.New
{
    /// <summary>
    /// Support for logging and for batches of logs.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Verbose logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        void LogVerbose(
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Information logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        void LogInformation(
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Warning logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        void LogWarning(
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Error logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        void LogError(
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Critical logging of <paramref name="message"/> and optional <paramref name="exception"/>.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        void LogCritical(
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Safe logging of a message. Will check for errors, but never throw an exception. If the log can't be made with the chosen logger, a fallback log will be created.
        /// </summary>
        /// <param name="severityLevel">The severity level for this log.</param>
        /// <param name="message">The message to print.</param>
        /// <param name="exception">An optional exception that will have it's information incorporated in the message.</param>
        /// <param name="memberName">Method or property name of the caller</param>
        /// <param name="filePath">Full path of the source file that contains the caller. This is the file path at compile time.</param>
        /// <param name="lineNumber">Line number in the source file at which the method is called</param>
        void LogOnLevel(
            LogSeverityLevel severityLevel,
            string message,
            Exception exception = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Start a new batch of logs. All the following logs will be saved internally and will not be activated until you call <see cref="Logger.ExecuteBatch"/>.
        /// </summary>
        /// <remarks>
        /// There are some cases where ExecuteBatch() will be called automatically:
        /// - If StartBatch() is called again
        /// - If the context for a new log compared with the current batch.
        /// </remarks>
        void StartBatch();

        /// <summary>
        /// Activate the logs that have been saved internally since the latest <see cref="Logger.StartBatch"/>.
        /// </summary>
        void ExecuteBatch();
    }
}