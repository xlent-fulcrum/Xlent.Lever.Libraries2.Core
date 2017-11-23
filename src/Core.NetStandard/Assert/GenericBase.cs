using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.Misc;

namespace Xlent.Lever.Libraries2.Core.Assert
{ 
    /// <summary>
    /// A generic class for asserting things that the programmer thinks is true. Generic in the meaning that a parameter says what exception that should be thrown when an assumption is false.
    /// </summary>
    internal static class GenericBase<TException>
        where TException : FulcrumException
    {
        [StackTraceHidden]
        public static void ThrowException(string message, string errorLocation = null)
        {
            var exception = (TException)Activator.CreateInstance(typeof(TException), message);
            exception.ErrorLocation = errorLocation ?? Environment.StackTrace;
            if (exception is FulcrumAssertionFailedException ||
                exception is FulcrumContractException)
            {
                Log.LogError("An unexpected internal error resulted in an exception.", exception);
            }
            throw exception;
        }
    }
}
