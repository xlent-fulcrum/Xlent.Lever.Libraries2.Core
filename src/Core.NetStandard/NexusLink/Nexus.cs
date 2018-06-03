using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Guards;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.Logging.New;

namespace Xlent.Lever.Libraries2.Core.NexusLink
{
    /// <summary>
    /// A collection of utilities that are static in their nature, but we prefer to implement then as proper classes and make them easily accessible from one static point.
    /// </summary>
    public static class Nexus
    {
        static Nexus()
        {
            // Logger
            Logger = new Logger();
            // Assert
            Assert.Critical = new Guard(LogSeverityLevel.Critical, typeof(FulcrumAssertionFailedException));
            Assert.Warning = new Guard(LogSeverityLevel.Warning);
            // Require
            Require.Internal = new Guard(LogSeverityLevel.Critical, typeof(FulcrumAssertionFailedException));
            Require.Public = new Guard(LogSeverityLevel.Critical, typeof(FulcrumContractException));
            Require.Api = new Guard(LogSeverityLevel.Critical, typeof(FulcrumServiceContractException));
            // Expect
            Expect.Internal = new Guard(LogSeverityLevel.Critical, typeof(FulcrumAssertionFailedException));
            Expect.Public = new Guard(LogSeverityLevel.Critical, typeof(FulcrumAssertionFailedException));
            Expect.Api = new Guard(LogSeverityLevel.Critical, typeof(FulcrumResourceContractException));
        }

        /// <summary>
        /// Methods for logging messages with different <see cref="LogSeverityLevel">severity levels</see>.
        /// </summary>
        public static ILogger Logger { get; set; }

        /// <summary>
        /// Properties for verifying that the programmers assumptions are correct. Works both as documentation and as a verification.
        /// </summary>
        public struct Assert
        {

            /// <summary>
            /// Unforgiving verification; logs as a critical error and trows an exception. 
            /// </summary>
            /// <remarks>If an assertion fails, it will be logged as <see cref="LogSeverityLevel.Critical"/> and the exception <see cref="FulcrumAssertionFailedException"/> will be thrown.</remarks>
            public static IGuard Critical { get; set; }

            /// <summary>
            /// A mild version of assertion verification. Will not throw any exception, it will only log a <see cref="LogSeverityLevel.Warning"/>
            /// </summary>
            /// <remarks>If a requirement fails, it will be logged as <see cref="LogSeverityLevel.Critical"/> and the exception <see cref="FulcrumServiceContractException"/> will be thrown.</remarks>
            public static IGuard Warning { get; set; }
        }

        /// <summary>
        /// Properties for verifying that a method contract is respected by the caller.
        /// </summary>
        public struct Require
        {
            /// <summary>
            /// Verify that a method contract is respected for method calls within the same ".DLL".
            /// </summary>
            /// <remarks>If a requirement fails, it will be logged as <see cref="LogSeverityLevel.Critical"/> and the exception <see cref="FulcrumAssertionFailedException"/> will be thrown.</remarks>
            public static IGuard Internal { get; set; }

            /// <summary>
            /// Verify that a method contract is respected by the caller. Please note that this is used in truly "public" methods, i.e. calls that crosses ".DLL" boundaries. For methods within the same ".DLL", you should use <see cref="Internal"/>.
            /// </summary>
            /// <remarks>If a requirement fails, it will be logged as <see cref="LogSeverityLevel.Critical"/> and the exception <see cref="FulcrumContractException"/> will be thrown.</remarks>
            public static IGuard Public { get; set; }

            /// <summary>
            /// Verify that a method contract is respected by the caller. Please note that this is only used for API:s,
            /// dealing with errors caused by someone outside our ".EXE". For methods within the same ".EXE", you should use <see cref="Public"/> or <see cref="Internal"/>.
            /// </summary>
            /// <remarks>If a requirement fails, it will be logged as <see cref="LogSeverityLevel.Critical"/> and the exception <see cref="FulcrumServiceContractException"/> will be thrown.</remarks>
            public static IGuard Api { get; set; }
        }

        /// <summary>
        /// Properties for verifying that the result from a method call respects the contract.
        /// </summary>
        public struct Expect
        {
            /// <summary>
            /// For calls within the same ".DLL".
            /// </summary>
            /// <remarks>If an expectation fails, it will be logged as <see cref="LogSeverityLevel.Critical"/> and the exception <see cref="FulcrumAssertionFailedException"/> will be thrown.</remarks>
            public static IGuard Internal { get; set; }

            /// <summary>
            /// For calls that crosses ".DLL" boundaries. For methods within the same ".DLL", you should use <see cref="Internal"/>.
            /// </summary>
            /// <remarks>If an expectation fails, it will be logged as <see cref="LogSeverityLevel.Critical"/> and the exception <see cref="FulcrumAssertionFailedException"/> will be thrown.</remarks>
            public static IGuard Public { get; set; }

            /// <summary>
            /// For calls outside our ".EXE". For methods within the same ".EXE", you should use <see cref="Public"/> or <see cref="Internal"/>.
            /// </summary>
            /// <remarks>If an expectation fails, it will be logged as <see cref="LogSeverityLevel.Critical"/> and the exception <see cref="FulcrumServiceContractException"/> will be thrown.</remarks>
            public static IGuard Api { get; set; }
        }
    }
}
