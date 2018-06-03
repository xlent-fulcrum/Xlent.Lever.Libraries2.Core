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
    public class Guard : IGuard
    {
        /// <summary>
        /// The guard to use for all guard constructors
        /// </summary>
        protected static IGuard ConstructorGuard { get; }

        /// <summary>
        /// A guard for the guards
        /// </summary>
        protected static IGuard InternalGuard { get; }

        /// <summary>
        /// When 
        /// </summary>
        public Type ExceptionTypeToThrow { get; private set; }

        /// <summary>
        /// When a guard fails, shouldwe log this as an error?
        /// </summary>
        public LogSeverityLevel LogAsLevel { get; private set; }

        static Guard()
        {
            InternalGuard = new Guard
            {
                LogAsLevel = LogSeverityLevel.Critical,
                ExceptionTypeToThrow = typeof(FulcrumContractException)
            };
            ConstructorGuard = new Guard
            {
                LogAsLevel = LogSeverityLevel.Critical,
                ExceptionTypeToThrow = null
            };
        }

        /// <summary>
        /// Internal empty constructor, only used by the static constructor.
        /// </summary>
        private Guard()
        {
        }

        /// <summary>
        /// Constructor with settings for how to behave on guards that are not fulfilled.
        /// </summary>
        /// <param name="exceptionTypeToThrow">The exception type to throw. Null means the no exceptions are thrown.</param>
        /// <param name="logAsLevel">The severity level to use for logging. Use <see cref="LogSeverityLevel.None"/> for no logging.</param>
        public Guard(LogSeverityLevel logAsLevel, Type exceptionTypeToThrow = null)
        {
            if (exceptionTypeToThrow == null)
            {
                ConstructorGuard.AreNotEqual(logAsLevel,
                    LogSeverityLevel.None, $"Instantiated an object of type {typeof(Guard).FullName} with no logging and no exception. Considered critical, as the consequence is that all guards will effectively do nothing.");
            }
            else
            {
                ConstructorGuard.IsAssignableTo(exceptionTypeToThrow,
                    typeof(Exception), $"Instantiated an object of type {typeof(Guard).FullName} with an exception type ({exceptionTypeToThrow.FullName}) that does not inherit from {nameof(Exception)}. The consequence is that no exceptions will be thrown for the guards.");
            }
            LogAsLevel = logAsLevel;
            ExceptionTypeToThrow = exceptionTypeToThrow;
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
            customMessage = customMessage ?? $"Expected the value to be TRUE.";
            MaybeLogAndOrThrow(customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsFalse(bool value, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            if (!value) return;
            customMessage = customMessage ?? $"Expected the value to be TRUE.";
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
            customMessage = customMessage ?? $"Expected the value to be NULL.";
            IsTrue(value == null, customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public virtual void IsNotNull(object value, string customMessage = null, int lineNumber = 0, string filePath = "",
            string memberName = "")
        {
            customMessage = customMessage ?? $"The value was unexpectedly NULL.";
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
            customMessage = customMessage ?? $"The value was expected to be the same as the default value for {typeof(T).Name} ({value}).";
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
            customMessage = customMessage ?? $"The value was unexpectedly the same as the default value for {typeof(T).Name} ({value}).";
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
            customMessage = customMessage ?? $"Expected the value to be null, empty or only contain white space.";
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
            customMessage = customMessage ?? $"Expected the value to be not be null, not be empty and contain other characters than white space.";
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
            customMessage = customMessage ?? $"The value '{value}' was expected to be equal to '{expectedValue}').";
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
            customMessage = customMessage ?? $"The value was not expected to be equal to '{unexpectedValue}').";
            IsTrue(!Equals(unexpectedValue, value), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public void IsAssignableTo(Type value, Type expectedType,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (value == null) return;
            InternalGuard.IsNotNull(expectedType, $"Parameter {nameof(expectedType)} must not be null");
            customMessage = customMessage ?? $"The value of type {value.FullName} was expected to be an instance of {expectedType.FullName}.";
            IsTrue(expectedType.IsAssignableFrom(value), customMessage, lineNumber, filePath, memberName);
        }

        /// <inheritdoc />
        [StackTraceHidden]
        public void IsNotAssignableTo(Type value, Type unexpectedType,
            string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (value == null) return;
            InternalGuard.IsNotNull(unexpectedType, $"Parameter {nameof(unexpectedType)} must not be null");
            customMessage = customMessage ?? $"The value was not expected to be an instance of {unexpectedType.FullName}.";
            InternalGuard.IsNotNull(unexpectedType, $"The parameter {nameof(unexpectedType)} must not be null.", lineNumber, filePath, memberName);
            IsTrue(!unexpectedType.IsAssignableFrom(value), customMessage, lineNumber, filePath, memberName);
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
            InternalGuard.IsNotNull(expectedType, $"Parameter {nameof(expectedType)} must not be null");
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
            InternalGuard.IsNotNull(unexpectedType, $"Parameter {nameof(unexpectedType)} must not be null");
            customMessage = customMessage ?? $"The value was not expected to be an instance of {unexpectedType.FullName}.";
            InternalGuard.IsNotNull(unexpectedType, $"The parameter {nameof(unexpectedType)} must not be null.", lineNumber, filePath, memberName);
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
            InternalGuard.IsNotNull(greaterValue, $"The parameter {nameof(greaterValue)} must not be null.", lineNumber, filePath, memberName);
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
            InternalGuard.IsNotNull(greaterOrEqualValue, $"The parameter {nameof(greaterOrEqualValue)} must not be null.", lineNumber, filePath, memberName);
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
            InternalGuard.IsNotNull(lesserValue, $"The parameter {nameof(lesserValue)} must not be null.", lineNumber, filePath, memberName);
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
            InternalGuard.IsNotNull(lesserOrEqualValue, $"The parameter {nameof(lesserOrEqualValue)} must not be null.", lineNumber, filePath, memberName);
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
            customMessage = customMessage ?? $"Expected the value ({value}) to match a regular expression ({regularExpression})";
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
            customMessage = customMessage ?? $"Expected the value ({value}) to not match a regular expression ({regularExpression})";
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
            if (!(value is IValidatable validatable)) return;
            // TODO: Refactor Validate to accept string customMessage = null, int lineNumber = 0, string filePath = "", string memberName = ""
            validatable.Validate($"{memberName} in {filePath} at line {lineNumber}");
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
            foreach (var value in values)
            {
                IsValid(value, customMessage, lineNumber, filePath, memberName);
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
            var exception = Activator.CreateInstance(ExceptionTypeToThrow, customMessage) as Exception;
            switch (exception)
            {
                case null:
                    return;
                case FulcrumException fulcrumException:
                    fulcrumException.ErrorLocation = $"{memberName} at line {lineNumber} in {filePath}";
                    break;
            }

            throw exception;
        }
    }

}
