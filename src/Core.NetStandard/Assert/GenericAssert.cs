using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Misc;

namespace Xlent.Lever.Libraries2.Core.Assert
{ 
    /// <summary>
    /// A generic class for asserting things that the programmer thinks is true. Generic in the meaning that a parameter says what exception that should be thrown when an assumption is false.
    /// </summary>
    internal static class GenericAssert<TException>
        where TException : FulcrumException
    {
        /// <summary>
        /// Will always fail. Used in parts of the errorLocation where we should never end up. E.g. a default case in a switch statement where all cases should be covered, so we should never end up in the default case.
        /// </summary>
        /// <param name="errorLocation">A unique errorLocation for this exact assertion.</param>
        /// <param name="message">A message that documents/explains this failure. This message should normally start with "Expected ...".</param>
        [StackTraceHidden]
        public static void Fail(string errorLocation, string message)
        {
            InternalContract.RequireNotNullOrWhitespace(message, nameof(message));
            GenericBase<TException>.ThrowException(message, errorLocation);
        }
        /// <summary>
        /// Will always fail. Used in parts of the errorLocation where we should never end up. E.g. a default case in a switch statement where all cases should be covered, so we should never end up in the default case.
        /// </summary>
        /// <param name="message">A message that documents/explains this failure. This message should normally start with "Expected ...".</param>
        [StackTraceHidden]
        public static void Fail(string message)
        {
            InternalContract.RequireNotNullOrWhitespace(message, nameof(message));
            GenericBase<TException>.ThrowException(message);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is true.
        /// </summary>
        [StackTraceHidden]
        public static void IsTrue(bool value, string errorLocation = null, string customMessage = null)
        {
            if (value) return;
            GenericBase<TException>.ThrowException(customMessage ?? "Expected value to be true.", errorLocation);
        }

        /// <summary>
        /// Verify that the result of <paramref name="expression"/> is true.
        /// </summary>
        [Obsolete("Please notify the Fulcrum team if you use this assertion method. We intend to remove it.")]
        [StackTraceHidden]
        public static void IsTrue(Expression<Func<bool>> expression, string errorLocation = null, string customMessage = null)
        {
            var value = expression.Compile()();
            if (value) return;
            GenericBase<TException>.ThrowException(customMessage ?? $"Expected '{expression.Body} to be true.", errorLocation);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is null.
        /// </summary>
        [StackTraceHidden]
        public static void IsNull(object value, string errorLocation = null, string customMessage = null)
        {
            if (value == null) return;
            GenericBase<TException>.ThrowException(customMessage ?? $"Expected value ({value}) to be null.", errorLocation);
        }

        /// <summary>
        /// Verify that the result of <paramref name="expression"/> is null.
        /// </summary>
        [Obsolete("Please notify the Fulcrum team if you use this assertion method. We intend to remove it.")]
        [StackTraceHidden]
        public static void IsNull(Expression<Func<object>> expression, string errorLocation = null, string customMessage = null)
        {
            var value = expression.Compile()();
            if (value == null) return;
            GenericBase<TException>.ThrowException(customMessage ?? $"Expected '{expression.Body}' ({value}) to be null.", errorLocation);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is not null.
        /// </summary>
        [StackTraceHidden]
        public static void IsNotNull(object value, string errorLocation = null, string customMessage = null)
        {
            if (value != null) return;
            GenericBase<TException>.ThrowException(customMessage ?? "Did not expect value to be null.", errorLocation);
        }

        /// <summary>
        /// Verify that the result of <paramref name="expression"/> is null.
        /// </summary>
        [Obsolete("Please notify the Fulcrum team if you use this assertion method. We intend to remove it.")]
        [StackTraceHidden]
        public static void IsNotNull(Expression<Func<object>> expression, string errorLocation = null, string customMessage = null)
        {
            var value = expression.Compile()();
            if (value != null) return;
            GenericBase<TException>.ThrowException(customMessage ?? $"Did not expect '{expression.Body}' to be null.", errorLocation);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is not the default value for that type.
        /// </summary>
        [StackTraceHidden]
        public static void IsNotDefaultValue<T>(T value, string errorLocation = null, string customMessage = null)
        {
            if (!value.Equals(default(T))) return;
            GenericBase<TException>.ThrowException(customMessage ?? $"Did not expect value to be default value ({default(T)}).", errorLocation);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is not null, not empty and has other characters than white space.
        /// </summary>
        [StackTraceHidden]
        public static void IsNotNullOrWhiteSpace(string value, string errorLocation = null, string customMessage = null)
        {
            if (!string.IsNullOrWhiteSpace(value)) return;
            GenericBase<TException>.ThrowException(customMessage ?? $"Did not expect value ({value}) to be null, empty or only contain whitespace.", errorLocation);
        }

        /// <summary>
        /// Verify that the result of <paramref name="expression"/> is not null, not empty and contains other characters than white space.
        /// </summary>
        [Obsolete("Please notify the Fulcrum team if you use this assertion method. We intend to remove it.")]
        [StackTraceHidden]
        public static void IsNotNullOrWhiteSpace(Expression<Func<string>> expression, string errorLocation = null, string customMessage = null)
        {
            var value = expression.Compile()();
            if (!string.IsNullOrWhiteSpace(value)) return;
            GenericBase<TException>.ThrowException(customMessage ?? $"Did not expect '{expression.Body}' ({value}) to be null, empty or only contain whitespace.", errorLocation);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is equal to <paramref name="expectedValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void AreEqual(object expectedValue, object actualValue, string errorLocation = null, string customMessage = null)
        {
            if (Equals(expectedValue, actualValue)) return;
            GenericBase<TException>.ThrowException(customMessage ?? $"Expected ({actualValue}) to be equal to ({expectedValue}).", errorLocation);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is not equal to <paramref name="expectedValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void AreNotEqual(object expectedValue, object actualValue, string errorLocation = null, string customMessage = null)
        {
            if (!Equals(expectedValue, actualValue)) return;
            GenericBase<TException>.ThrowException(customMessage ?? $"Expected ({actualValue}) to not be equal to ({expectedValue}).", errorLocation);
        }

        /// <summary>
        /// Verify that the result of <paramref name="expression"/> is equal to <paramref name="expectedValue"/>.
        /// </summary>
        [Obsolete("Please notify the Fulcrum team if you use this assertion method. We intend to remove it.")]
        [StackTraceHidden]
        public static void AreEqual(object expectedValue, Expression<Func<string>> expression, string errorLocation = null, string customMessage = null)
        {
            var actualValue = expression.Compile()();
            if (Equals(expectedValue, actualValue)) return;
            GenericBase<TException>.ThrowException(customMessage ?? $"Expected '{expression.Body}' ({actualValue}) to be equal to ({expectedValue}).", errorLocation);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is less than to <paramref name="greaterValue"/>.
        /// </summary>
        public static void IsLessThan<T>(T greaterValue, T actualValue, string errorLocation = null, string customMessage = null)
            where T : IComparable<T>
        {
            InternalContract.RequireNotNull(greaterValue, nameof(greaterValue));
            InternalContract.RequireNotNull(actualValue, nameof(actualValue));
            var message = customMessage ?? $"Expected ({actualValue}) to be less than ({greaterValue}).";
            IsTrue(actualValue.CompareTo(greaterValue) < 0, errorLocation, message);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is less than or equal to <paramref name="greaterOrEqualValue"/>.
        /// </summary>
        public static void IsLessThanOrEqualTo<T>(T greaterOrEqualValue, T actualValue, string errorLocation = null, string customMessage = null)
            where T : IComparable<T>
        {
            InternalContract.RequireNotNull(greaterOrEqualValue, nameof(greaterOrEqualValue));
            InternalContract.RequireNotNull(actualValue, nameof(actualValue));
            var message = customMessage ?? $"Expected ({actualValue}) to be less or equal to ({greaterOrEqualValue}).";
            IsTrue(actualValue.CompareTo(greaterOrEqualValue) <= 0, errorLocation, message);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is greater than <paramref name="lesserValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void IsGreaterThan<T>(T lesserValue, T actualValue, string errorLocation = null, string customMessage = null)
            where T : IComparable<T>
        {
            InternalContract.RequireNotNull(lesserValue, nameof(lesserValue));
            InternalContract.RequireNotNull(actualValue, nameof(actualValue));
            var message = customMessage ?? $"Expected ({actualValue}) to be greater than ({lesserValue}).";
            IsTrue(actualValue.CompareTo(lesserValue) > 0, errorLocation, message);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is greater than or equal to <paramref name="lesserOrEqualValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void IsGreaterThanOrEqualTo<T>(T lesserOrEqualValue, T actualValue, string errorLocation = null, string customMessage = null)
            where T : IComparable<T>
        {
            InternalContract.RequireNotNull(lesserOrEqualValue, nameof(lesserOrEqualValue));
            InternalContract.RequireNotNull(actualValue, nameof(actualValue));
            var message = customMessage ?? $"Expected ({actualValue}) to be greater than or equal to ({lesserOrEqualValue}).";
            IsTrue(actualValue.CompareTo(lesserOrEqualValue) >= 0, errorLocation, message);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is null or matches the regular expression <paramref name="regularExpression"/>.
        /// </summary>
        [StackTraceHidden]
        public static void MatchesRegExp(string regularExpression, string value, string errorLocation = null, string customMessage = null)
        {
            if (value == null) return;
            InternalContract.RequireNotNull(regularExpression, nameof(regularExpression));
            var message = customMessage ?? $"Expected ({value}) to match regular expression ({regularExpression}).";
            IsTrue(Regex.IsMatch(value, regularExpression), errorLocation, message);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is null or matches the regular expression <paramref name="regularExpression"/>.
        /// </summary>
        [StackTraceHidden]
        public static void MatchesNotRegExp(string regularExpression, string value, string errorLocation = null, string customMessage = null)
        {
            if (value == null) return;
            InternalContract.RequireNotNull(regularExpression, nameof(regularExpression));
            var message = customMessage ?? $"Expected ({value}) to not match regular expression ({regularExpression}).";
            IsTrue(!Regex.IsMatch(value, regularExpression), errorLocation, message);
        }
    }
}
