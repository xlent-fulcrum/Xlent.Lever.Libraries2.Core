using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Guards;
using Xlent.Lever.Libraries2.Core.Misc;
using Xlent.Lever.Libraries2.Core.NexusLink;
// ReSharper disable ExplicitCallerInfoArgument

namespace Xlent.Lever.Libraries2.Core.Assert
{
    /// <summary>
    /// A class for verifying method contracts. Will throw <see cref="FulcrumContractException"/> if the contract is broken.
    /// </summary>
    public static class InternalContract
    {
        private static readonly Lazy<IContractGuard> LazyGuard =
            new Lazy<IContractGuard>(() => Nexus.Require.Internal);

        private static IContractGuard Guard => LazyGuard.Value;

        /// <summary>
        /// Verify that <paramref name="expression"/> return true, when applied to <paramref name="parameterValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void Require<TParameter>(TParameter parameterValue,
            Expression<Func<TParameter, bool>> expression, string parameterName,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            var isTrue = expression.Compile()(parameterValue);
            var condition = expression.Body.ToString();
            condition = condition.Replace(expression.Parameters.First().Name, parameterName);
            var message = $"Contract violation: {parameterName} ({parameterValue}) is required to fulfill {condition}.";
            Guard.IsTrue(isTrue, message, parameterName, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is not null.
        /// </summary>
        [StackTraceHidden]
        public static void RequireNotNull(object parameterValue, string parameterName, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsNotNull(parameterValue, parameterName, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is not the default parameterValue for this type.
        /// </summary>
        [StackTraceHidden]
        public static void RequireNotDefaultValue<TParameter>(TParameter parameterValue, string parameterName, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsNotDefaultValue(parameterValue, parameterName, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is not null, not empty and contains other characters than white space.
        /// </summary>
        [StackTraceHidden]
        public static void RequireNotNullOrWhitespace(string parameterValue, string parameterName, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsNotNullOrWhiteSpace(parameterValue, parameterName, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// If <paramref name="parameterValue"/> is not null, then call the FulcrumValidate() method of that type.
        /// </summary>
        [StackTraceHidden]
        public static void RequireValidated(object parameterValue, string parameterName, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsValid(parameterValue, parameterName, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// If <paramref name="parameterValues"/> is not null, then call the Validate() method for each item.
        /// </summary>
        [StackTraceHidden]
        public static void RequireValidated(IEnumerable<object> parameterValues, string parameterName, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsValid(parameterValues, parameterName, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="mustBeTrue"/> really is true.
        /// </summary>
        [StackTraceHidden]
        public static void Require(bool mustBeTrue, string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Nexus.Require.Public.IsNotNullOrWhiteSpace(message, nameof(message));
            Guard.IsTrue(mustBeTrue, message, null, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is less than to <paramref name="greaterValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void RequireLessThan<T>(T greaterValue, T parameterValue, string parameterName, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            RequireNotNull(greaterValue, nameof(greaterValue));
            RequireNotNull(parameterValue, nameof(parameterValue));
            RequireNotNull(parameterName, nameof(parameterName));
            GenericContract<FulcrumContractException>.RequireLessThan(greaterValue, parameterValue, parameterName, customMessage);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is less than or equal to <paramref name="greaterOrEqualValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void RequireLessThanOrEqualTo<T>(T greaterOrEqualValue, T parameterValue, string parameterName, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            RequireNotNull(greaterOrEqualValue, nameof(greaterOrEqualValue));
            RequireNotNull(parameterValue, nameof(parameterValue));
            RequireNotNull(parameterName, nameof(parameterName));
            GenericContract<FulcrumContractException>.RequireLessThanOrEqualTo(greaterOrEqualValue, parameterValue, parameterName, customMessage);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is greater than <paramref name="lesserValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void RequireGreaterThan<T>(T lesserValue, T parameterValue, string parameterName, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            RequireNotNull(lesserValue, nameof(lesserValue));
            RequireNotNull(parameterValue, nameof(parameterValue));
            RequireNotNull(parameterName, nameof(parameterName));
            GenericContract<FulcrumContractException>.RequireGreaterThan(lesserValue, parameterValue, parameterName, customMessage);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is greater than or equal to <paramref name="lesserOrEqualValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void RequireGreaterThanOrEqualTo<T>(T lesserOrEqualValue, T parameterValue, string parameterName, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            RequireNotNull(lesserOrEqualValue, nameof(lesserOrEqualValue));
            RequireNotNull(parameterValue, nameof(parameterValue));
            RequireNotNull(parameterName, nameof(parameterName));
            GenericContract<FulcrumContractException>.RequireGreaterThanOrEqualTo(lesserOrEqualValue, parameterValue, parameterName, customMessage);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is null or matches the regular expression <paramref name="regularExpression"/>.
        /// </summary>
        [StackTraceHidden]
        public static void MatchesRegExp(string regularExpression, string parameterValue, string parameterName, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            RequireNotNullOrWhitespace(regularExpression, nameof(regularExpression));
            RequireNotNull(parameterName, nameof(parameterName));
            GenericContract<FulcrumContractException>.RequireMatchesRegExp(regularExpression, parameterValue, parameterName, customMessage);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is null or not matches the regular expression <paramref name="regularExpression"/>.
        /// </summary>
        [StackTraceHidden]
        public static void MatchesNotRegExp(string regularExpression, string value, string errorLocation, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            RequireNotNullOrWhitespace(regularExpression, nameof(regularExpression));
            GenericContract<FulcrumContractException>.RequireMatchesNotRegExp(regularExpression, value, errorLocation, customMessage);
        }

        /// <summary>
        /// Always fail, with the given <paramref name="message"/>.
        /// </summary>
        [StackTraceHidden]
        public static void Fail(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            RequireNotNull(message, nameof(message));
            GenericContract<FulcrumContractException>.Fail(message);
        }
    }
}
