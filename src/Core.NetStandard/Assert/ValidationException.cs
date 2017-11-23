using System;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Assert
{
    /// <summary>
    /// The base class for all Fulcrum exceptions
    /// </summary>
    public class ValidationException : FulcrumException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationException() : this(null, (Exception)null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationException(string message) : this(message, (Exception)null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
