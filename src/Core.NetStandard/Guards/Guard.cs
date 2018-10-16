using System;
using System.Collections.Generic;
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
    public class Guard : IGuard
    {
        private static readonly Lazy<IGuard> LazyConstructorGuard =
            new Lazy<IGuard>(() => new Guard(LogSeverityLevel.Critical, typeof(FulcrumContractException), false));

        /// <summary>
        /// The guard to use for all guard constructors
        /// </summary>
        protected static IGuard ConstructorGuard => LazyConstructorGuard.Value;

        private static readonly Lazy<IContractGuard> LazyInternalGuard =
            new Lazy<IContractGuard>(() => new ContractGuard(LogSeverityLevel.Critical, null, false));

        /// <summary>
        /// A guard for the guards
        /// </summary>
        protected static IContractGuard InternalGuard => LazyInternalGuard.Value;

        /// <summary>
        /// When 
        /// </summary>
        public Type ExceptionTypeToThrow { get; protected internal set; }

        /// <summary>
        /// When a guard fails, should we log this as an error?
        /// </summary>
        public LogSeverityLevel LogAsLevel { get; protected internal set; }

        /// <summary>
        /// Constructor with settings for how to behave on guards that are not fulfilled.
        /// </summary>
        /// <param name="exceptionTypeToThrow">The exception type to throw. Null means the no exceptions are thrown.</param>
        /// <param name="logAsLevel">The severity level to use for logging. Use <see cref="LogSeverityLevel.None"/> for no logging.</param>
        /// <param name="verifyParameters">True means that we should verify the parameters. Should be true for external calls and false for internal calls.</param>
        public Guard(LogSeverityLevel logAsLevel, Type exceptionTypeToThrow, bool verifyParameters)
        {
            if (verifyParameters)
            {
                if (exceptionTypeToThrow == null)
                {
                    ConstructorGuard.AreNotEqual(logAsLevel,LogSeverityLevel.None,
                        $"Instantiated an object of type {typeof(Guard).FullName} with no logging and no exception. Considered critical, as the consequence is that all guards will effectively do nothing.");
                }
                else
                {
                    ConstructorGuard.IsAssignableTo(exceptionTypeToThrow,
                        typeof(Exception),
                        $"Instantiated an object of type {typeof(Guard).FullName} with an exception type ({exceptionTypeToThrow.FullName}) that does not inherit from {nameof(Exception)}. The consequence is that no exceptions will be thrown for the guards.");
                }
            }

            LogAsLevel = logAsLevel;
            ExceptionTypeToThrow = exceptionTypeToThrow;
        }

        /// <summary>
        /// Constructor with settings for how to behave on guards that are not fulfilled.
        /// </summary>
        /// <param name="exceptionTypeToThrow">The exception type to throw. Null means the no exceptions are thrown.</param>
        /// <param name="logAsLevel">The severity level to use for logging. Use <see cref="LogSeverityLevel.None"/> for no logging.</param>
        public Guard(LogSeverityLevel logAsLevel, Type exceptionTypeToThrow = null)
            : this(logAsLevel, exceptionTypeToThrow, true)
        {
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void Fail(
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            customMessage = customMessage ?? $"Reached a point in the code that should not be reached.";
            MaybeLogAndOrThrow(customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsTrue(bool value,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (value) return;
            customMessage = customMessage ?? $"Expected the value ({false}) to be true.";
            MaybeLogAndOrThrow(customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsFalse(bool value, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            if (!value) return;
            customMessage = customMessage ?? $"Expected the value ({true}) to be false.";
            MaybeLogAndOrThrow(customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsNull(object value,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            customMessage = customMessage ?? $"Expected the value ({value}) to be null.";
            IsTrue(value == null, customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsNotNull(object value, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            customMessage = customMessage ?? $"Did not expect the value ({value}) to be null.";
            IsTrue(value != null, customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsDefaultValue<T>(T value,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            customMessage = customMessage ?? $"The value ({value}) was expected to be the same as the default value for {typeof(T).Name} ({default(T)}).";
            IsTrue(Equals(value, default(T)), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsNotDefaultValue<T>(T value,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            customMessage = customMessage ?? $"Did not expect the value ({value}) to be the same as the default value for {typeof(T).Name} ({default(T)}).";
            IsTrue(!Equals(value, default(T)), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsNullOrWhiteSpace(string value,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            customMessage = customMessage ?? $"Expected the value ({value}) to be null, empty or only contain white space.";
            IsTrue(string.IsNullOrWhiteSpace(value), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsNotNullOrWhiteSpace(string value,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            customMessage = customMessage ?? $"Did not expect the value ({value}) to be null, empty or only contain white space.";
            IsTrue(!string.IsNullOrWhiteSpace(value), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void AreEqual(object value, object expectedValue,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            customMessage = customMessage ?? $"The value ({value}) was expected to be equal to '{expectedValue}').";
            IsTrue(Equals(expectedValue, value), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void AreNotEqual(object value, object unexpectedValue,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            customMessage = customMessage ?? $"The value ({value}) was not expected to be equal to '{unexpectedValue}').";
            IsTrue(!Equals(unexpectedValue, value), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public void IsAssignableTo(Type type, Type expectedType,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (type == null) return;
            InternalGuard.IsNotNull(expectedType, nameof(expectedType), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"The type {type.FullName} was expected to be assignable to {expectedType.FullName}.";
            IsTrue(expectedType.IsAssignableFrom(type), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public void IsNotAssignableTo(Type type, Type unexpectedType,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (type == null) return;
            InternalGuard.IsNotNull(unexpectedType, nameof(unexpectedType), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"The type {type.FullName} was not expected to be assignable to {unexpectedType.FullName}.";
            IsTrue(!unexpectedType.IsAssignableFrom(type), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public void IsInstanceOf(object value, Type expectedType,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (value == null) return;
            InternalGuard.IsNotNull(expectedType, nameof(expectedType), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"The value of type {value.GetType().FullName} was expected to be an instance of {expectedType.FullName}.";
            IsTrue(expectedType.IsInstanceOfType(value), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public void IsNotInstanceOf(object value, Type unexpectedType,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (value == null) return;
            InternalGuard.IsNotNull(unexpectedType, nameof(unexpectedType), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"The value of type {value.GetType().FullName} was not expected to be an instance of {unexpectedType.FullName}.";
            IsTrue(!unexpectedType.IsInstanceOfType(value), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsLessThan<T>(T value, T greaterValue,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            if (value == null) return;
            InternalGuard.IsNotNull(greaterValue, nameof(greaterValue), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"Expected the value ({value}) to be < {greaterValue}";
            IsTrue(value.CompareTo(greaterValue) < 0, customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsLessThanOrEqualTo<T>(T value, T greaterOrEqualValue,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            if (value == null) return;
            InternalGuard.IsNotNull(greaterOrEqualValue, nameof(greaterOrEqualValue), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"Expected the value ({value}) to be <= {greaterOrEqualValue}";
            IsTrue(value.CompareTo(greaterOrEqualValue) <= 0, customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsGreaterThan<T>(T value, T lesserValue,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            if (value == null) return;
            InternalGuard.IsNotNull(lesserValue, nameof(lesserValue), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"Expected the value ({value}) to be > {lesserValue}";
            IsTrue(value.CompareTo(lesserValue) > 0, customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsGreaterThanOrEqualTo<T>(T value, T lesserOrEqualValue,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            if (value == null) return;
            InternalGuard.IsNotNull(lesserOrEqualValue, nameof(lesserOrEqualValue), null, lineNumber, filePath, memberName);
            customMessage = customMessage ?? $"Expected the value ({value}) to be >= {lesserOrEqualValue}";
            IsTrue(value.CompareTo(lesserOrEqualValue) >= 0, customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsRegexMatch(string value, string regularExpression,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (value == null) return;
            if (regularExpression == null) return;
            customMessage = customMessage ?? $"Expected the value ({value}) to match the regular expression ({regularExpression})";
            IsTrue(Regex.IsMatch(value, regularExpression), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsNotRegexMatch(string value, string regularExpression,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (value == null) return;
            if (regularExpression == null) return;
            customMessage = customMessage ?? $"Did not expect the value ({value}) to match the regular expression ({regularExpression})";
            IsTrue(!Regex.IsMatch(value, regularExpression), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public void IsValid(object value,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
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
                Fail(customMessage, lineNumber, filePath, memberName);
            }
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public void IsValid(IEnumerable<object> values,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (values == null) return;
            foreach (object value in values)
            {
                IsValid(value, customMessage, lineNumber, filePath, memberName);
            }
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public void IsNotValid(object value,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (value == null) return;
            // TODO: Refactor Validate to accept string customMessage = null, int lineNumber = 0, string filePath = "", string memberName = ""
            if (!(value is IValidatable validatable)) return;
            try
            {
                validatable.Validate($"{memberName} in {filePath} at line {lineNumber}");
                customMessage = customMessage ?? $"Did not expect the value ({value}) to pass validation.";
                Fail(customMessage, lineNumber, filePath, memberName);
            }
            catch (ValidationException)
            {
                // As expected.
            }
        }

        [StackTraceHidden]
        private void MaybeLogAndOrThrow(string customMessage, int lineNumber, string filePath, string memberName)
        {
            if (LogAsLevel != LogSeverityLevel.None)
            {
                Nexus.Logger.LogOnLevel(LogAsLevel, customMessage, null, lineNumber, filePath, memberName);
            }
            if (ExceptionTypeToThrow == null) return;
            Exception exception = Activator.CreateInstance(ExceptionTypeToThrow, customMessage) as Exception;
            switch (exception)
            {
                case null:
                    return;
                case FulcrumException fulcrumException:
                    fulcrumException.ErrorLocation = $"{memberName}() (in {filePath}, line {lineNumber}";
                    break;
            }

            throw exception;
        }
    }

}
