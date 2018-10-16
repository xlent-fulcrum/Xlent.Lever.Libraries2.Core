using System;
using System.Collections.Generic;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Logging;

// ReSharper disable ExplicitCallerInfoArgument

namespace Xlent.Lever.Libraries2.Core.Guards
{
    /// <summary>
    /// A generic class for asserting things that the programmer thinks is true. Generic in the meaning that a parameter says what exception that should be thrown when an assumption is false.
    /// </summary>
    public class ContractGuard : IContractGuard
    {
        private readonly IGuard _guard;

        private static readonly Lazy<IContractGuard> LazyInternalGuard =
            new Lazy<IContractGuard>(() => new ContractGuard(LogSeverityLevel.Critical, null, false));

        /// <summary>
        /// A guard for the guards
        /// </summary>
        protected static IContractGuard InternalGuard => LazyInternalGuard.Value;

        /// <summary>
        /// Constructor with settings for how to behave on guards that are not fulfilled.
        /// </summary>
        /// <param name="exceptionTypeToThrow">The exception type to throw. Null means the no exceptions are thrown.</param>
        /// <param name="logAsLevel">The severity level to use for logging. Use <see cref="LogSeverityLevel.None"/> for no logging.</param>
        /// <param name="verifyParameters">True means that we should verify the parameters. Should be true for external calls and false for internal calls.</param>
        internal ContractGuard(LogSeverityLevel logAsLevel, Type exceptionTypeToThrow, bool verifyParameters)
        {
            _guard = new Guard(logAsLevel, exceptionTypeToThrow, verifyParameters);
        }

        /// <summary>
        /// Constructor with settings for how to behave on guards that are not fulfilled.
        /// </summary>
        /// <param name="exceptionTypeToThrow">The exception type to throw. Null means the no exceptions are thrown.</param>
        /// <param name="logAsLevel">The severity level to use for logging. Use <see cref="LogSeverityLevel.None"/> for no logging.</param>
        public ContractGuard(LogSeverityLevel logAsLevel, Type exceptionTypeToThrow = null)
        : this(logAsLevel, exceptionTypeToThrow, true)
        {
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

        /// <inheritdoc />
        public void IsTrue(bool value, string parameterName, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            customMessage = customMessage ?? $"Expected the value ({false}) to be true.";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsTrue(value, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsFalse(bool value, string parameterName, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            customMessage = customMessage ?? $"Expected the value ({true}) to be false.";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsFalse(value, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsNull(object value, string parameterName, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            customMessage = customMessage ?? $"Expected the value ({value}) to be null.";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsNull(value, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsNotNull(object value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            customMessage = customMessage ?? $"Did not expect the value ({value}) to be null.";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsNotNull(value, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsDefaultValue<T>(T value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            customMessage = customMessage ?? $"The value ({value}) was expected to be the same as the default value for {typeof(T).Name} ({default(T)}).";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsDefaultValue(value, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsNotDefaultValue<T>(T value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            customMessage = customMessage ?? $"Did not expect the value ({value}) to be the same as the default value for {typeof(T).Name} ({default(T)}).";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsNotDefaultValue(value, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsNullOrWhiteSpace(string value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            customMessage = customMessage ?? $"Expected the value ({value}) to be null, empty or only contain white space.";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsNullOrWhiteSpace(value, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsNotNullOrWhiteSpace(string value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            customMessage = customMessage ?? $"Did not expect the value ({value}) to be null, empty or only contain white space.";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsNotNullOrWhiteSpace(value, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void AreEqual(object value, object expectedValue, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            customMessage = customMessage ?? $"The value '{value}' was expected to be equal to '{expectedValue}').";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.AreEqual(value, expectedValue, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void AreNotEqual(object value, object unexpectedValue, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            customMessage = customMessage ?? $"The value ({value}) was not expected to be equal to '{unexpectedValue}').";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.AreNotEqual(value, unexpectedValue, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsAssignableTo(Type type, Type expectedType, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            if (type == null) return;
            InternalGuard.IsNotNull(expectedType, nameof(expectedType), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"The type {type.FullName} was expected to be assignable to {expectedType.FullName}.";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsAssignableTo(type, expectedType, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsNotAssignableTo(Type type, Type unexpectedType, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            if (type == null) return;
            InternalGuard.IsNotNull(unexpectedType, nameof(unexpectedType), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"The type {type.FullName} was not expected to be assignable to {unexpectedType.FullName}.";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsNotAssignableTo(type, unexpectedType, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsInstanceOf(object value, Type expectedType, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            if (value == null) return;
            InternalGuard.IsNotNull(expectedType, nameof(expectedType), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"The value of type {value.GetType().FullName} was expected to be an instance of {expectedType.FullName}.";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsInstanceOf(value, expectedType, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsNotInstanceOf(object value, Type unexpectedType, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            if (value == null) return;
            InternalGuard.IsNotNull(unexpectedType, nameof(unexpectedType), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"The value of type {value.GetType().FullName} was not expected to be an instance of {unexpectedType.FullName}.";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsNotInstanceOf(value, unexpectedType, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsLessThan<T>(T value, T greaterValue, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "") where T : IComparable<T>
        {
            if (value == null) return;
            InternalGuard.IsNotNull(greaterValue, nameof(greaterValue), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"Expected the value ({value}) to be < {greaterValue}";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsLessThan(value, greaterValue, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsLessThanOrEqualTo<T>(T value, T greaterOrEqualValue, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "") where T : IComparable<T>
        {
            if (value == null) return;
            InternalGuard.IsNotNull(greaterOrEqualValue, nameof(greaterOrEqualValue), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"Expected the value ({value}) to be <= {greaterOrEqualValue}";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsLessThanOrEqualTo(value, greaterOrEqualValue, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsGreaterThan<T>(T value, T lesserValue, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "") where T : IComparable<T>
        {
            if (value == null) return;
            InternalGuard.IsNotNull(lesserValue, nameof(lesserValue), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"Expected the value ({value}) to be > {lesserValue}";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsGreaterThan(value, lesserValue, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsGreaterThanOrEqualTo<T>(T value, T lesserOrEqualValue, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "") where T : IComparable<T>
        {
            if (value == null) return;
            InternalGuard.IsNotNull(lesserOrEqualValue, nameof(lesserOrEqualValue), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"Expected the value ({value}) to be >= {lesserOrEqualValue}";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsGreaterThanOrEqualTo(value, lesserOrEqualValue, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsRegexMatch(string value, string regularExpression, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            if (value == null) return;
            if (regularExpression == null) return;
            customMessage = customMessage ?? $"Expected the value ({value}) to match the regular expression ({regularExpression})";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsRegexMatch(value, regularExpression, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsNotRegexMatch(string value, string regularExpression, string parameterName, string customMessage = null,
            int lineNumber = 0, string filePath = "", string memberName = "")
        {
            if (value == null) return;
            if (regularExpression == null) return;
            customMessage = customMessage ?? $"Did not expect the value ({value}) to match the regular expression ({regularExpression})";
            var contractMessage = CreateContractMessage(parameterName, customMessage);
            _guard.IsNotRegexMatch(value, regularExpression, contractMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        public void IsValid(object value, string parameterName, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            if (value == null) return;
            // TODO: Refactor Validate to accept string customMessage = null, int lineNumber = 0, string filePath = "", string memberName = ""
            if (!(value is IValidatable validatable)) return;
            try
            {
                validatable.Validate($"{memberName} in {filePath} at line {lineNumber}");
            }
            catch (ValidationException e)
            {
                customMessage = customMessage ?? e.Message;
                var contractMessage = CreateContractMessage(parameterName, customMessage);
                _guard.Fail(contractMessage, lineNumber, filePath, memberName);
            }
        }

        /// <inheritdoc />
        public void IsValid(IEnumerable<object> values, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            if (values == null) return;
            foreach (object value in values)
            {
                IsValid(value, parameterName, customMessage, lineNumber, filePath, memberName);
            }
        }

        /// <inheritdoc />
        public void IsNotValid(object value, string parameterName, string customMessage = null, int lineNumber = 0,
            string filePath = "", string memberName = "")
        {
            if (value == null) return;
            // TODO: Refactor Validate to accept string customMessage = null, int lineNumber = 0, string filePath = "", string memberName = ""
            if (!(value is IValidatable validatable)) return;
            try
            {
                validatable.Validate($"{memberName} in {filePath} at line {lineNumber}");
                customMessage = customMessage ?? $"Did not expect the value ({value}) to pass validation.";
                var contractMessage = CreateContractMessage(parameterName, customMessage);
                _guard.Fail(contractMessage, lineNumber, filePath, memberName);
            }
            catch (ValidationException)
            {
                // As expected.
            }
        }
        private static string CreateContractMessage(string parameterName, string message)
        {
            return $"Contract violation for parameter {parameterName}: {message}";
        }
    }

}
