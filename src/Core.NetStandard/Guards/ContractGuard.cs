using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.Misc;
using Xlent.Lever.Libraries2.Core.NexusLink;

// ReSharper disable ExplicitCallerInfoArgument

namespace Xlent.Lever.Libraries2.Core.Guards
{
    /// <summary>
    /// A generic class for asserting things that the programmer thinks is true. Generic in the meaning that a parameter says what exception that should be thrown when an assumption is false.
    /// </summary>
    public class ContractGuard : IContractGuard
    {
        private IGuard _guard;

        /// <summary>
        /// A guard for the guards
        /// </summary>
        protected static IContractGuard InternalGuard { get; }

        /// <summary>
        /// Constructor with settings for how to behave on guards that are not fulfilled.
        /// </summary>
        /// <param name="exceptionTypeToThrow">The exception type to throw. Null means the no exceptions are thrown.</param>
        /// <param name="logAsLevel">The severity level to use for logging. Use <see cref="LogSeverityLevel.None"/> for no logging.</param>
        public ContractGuard(LogSeverityLevel logAsLevel, Type exceptionTypeToThrow = null)
        {
            _guard = new Guard(logAsLevel, exceptionTypeToThrow);
        }

        /// <inheritdoc />
        public void Fail(string parameterName, string message, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            InternalGuard.IsNotNullOrWhiteSpace(parameterName, nameof(parameterName));
            InternalGuard.IsNotNullOrWhiteSpace(message, nameof(message));
            var contractMessage = CreateContractMessage(parameterName, message);
            _guard.Fail(contractMessage, lineNumber, filePath, memberName);
        }

        private string CreateContractMessage(string parameterName, string message)
        {
            return $"Contract violation for parameter {parameterName}: {message}";
        }

        /// <inheritdoc />
        public void IsTrue(bool value, string message, int lineNumber = 0, string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsFalse(bool value, string message, int lineNumber = 0, string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsTrue(bool value, string parameterName, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsFalse(bool value, string parameterName, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsNull(object value, string parameterName, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsNotNull(object value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsDefaultValue<T>(T value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsNotDefaultValue<T>(T value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsNullOrWhiteSpace(string value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsNotNullOrWhiteSpace(string value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void AreEqual(object value, object expectedValue, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void AreNotEqual(object value, object unexpectedValue, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsAssignableTo(Type value, Type expectedType, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsNotAssignableTo(Type value, Type unexpectedType, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsInstanceOf(object value, Type expectedType, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsNotInstanceOf(object value, Type unexpectedType, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsLessThan<T>(T value, T greaterValue, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "") where T : IComparable<T>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsLessThanOrEqualTo<T>(T value, T greaterOrEqualValue, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "") where T : IComparable<T>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsGreaterThan<T>(T value, T lesserValue, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "") where T : IComparable<T>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsGreaterThanOrEqualTo<T>(T value, T lesserOrEqualValue, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "") where T : IComparable<T>
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsRegexMatch(string value, string regularExpression, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsNotRegexMatch(string value, string regularExpression, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsValid(object value, string parameterName, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsValid(IEnumerable<object> values, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IsNotValid(object value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            throw new NotImplementedException();
        }
    }

}
