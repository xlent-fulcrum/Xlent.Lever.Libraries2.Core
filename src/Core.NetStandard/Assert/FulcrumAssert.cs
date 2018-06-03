using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Guards;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.Misc;
using Xlent.Lever.Libraries2.Core.NexusLink;

// ReSharper disable ExplicitCallerInfoArgument
#pragma warning disable 1573

namespace Xlent.Lever.Libraries2.Core.Assert
{
    /// <summary>
    /// A class for asserting things that the programmer thinks is true. Works both as documentation and as verification that the programmers assumptions holds.
    /// </summary>
    public static class FulcrumAssert
    {
        private static readonly IGuard Guard;

        static FulcrumAssert()
        {
            Guard = Nexus.Assert.Critical;  //new Guard(LogSeverityLevel.Critical, typeof(FulcrumAssertionFailedException));
        }

        /// <summary>
        /// Will always fail. Used in parts of the errorLocation where we should never end up. E.g. a default case in a switch statement where all cases should be covered, so we should never end up in the default case.
        /// </summary>
        /// <param name="errorLocation">A unique errorLocation for this exact assertion. </param>
        /// <param name="message">A message that documents/explains this failure. This message should normally start with "Expected ...".</param>
        [StackTraceHidden]
        public static void Fail(string errorLocation, string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.Fail(message, lineNumber, filePath, memberName);
        }
        /// <summary>
        /// Will always fail. Used in parts of the errorLocation where we should never end up. E.g. a default case in a switch statement where all cases should be covered, so we should never end up in the default case.
        /// </summary>
        /// <param name="message">A message that documents/explains this failure. This message should normally start with "Expected ...".</param>
        [StackTraceHidden]
        public static void Fail(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.Fail(message, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is true.
        /// </summary>
        [StackTraceHidden]
        public static void IsTrue(bool value, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsTrue(value, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is null.
        /// </summary>
        [StackTraceHidden]
        public static void IsNull(object value, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsNull(value, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is not null.
        /// </summary>
        
        [StackTraceHidden]
        public static void IsNotNull(object value, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsNotNull(value, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is not the default value for that type.
        /// </summary>
        [StackTraceHidden]
        public static void IsNotDefaultValue<T>(T value, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsNotDefaultValue(value, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is not null, not empty and contains other characters than white space.
        /// </summary>
        [StackTraceHidden]
        public static void IsNotNullOrWhiteSpace(string value, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsNotNullOrWhiteSpace(value, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is equal to <paramref name="expectedValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void AreEqual(object expectedValue, object actualValue, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.AreEqual(actualValue, expectedValue, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is not equal to <paramref name="expectedValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void AreNotEqual(object expectedValue, object actualValue, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.AreNotEqual(actualValue, expectedValue, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is less than to <paramref name="greaterValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void IsLessThan<T>(T greaterValue, T actualValue, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            Guard.IsLessThan(actualValue, greaterValue, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is less than or equal to <paramref name="greaterOrEqualValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void IsLessThanOrEqualTo<T>(T greaterOrEqualValue, T actualValue, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            Guard.IsLessThanOrEqualTo(actualValue, greaterOrEqualValue, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is greater than <paramref name="lesserValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void IsGreaterThan<T>(T lesserValue, T actualValue, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            Guard.IsGreaterThan(actualValue, lesserValue, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="actualValue"/> is greater than or equal to <paramref name="lesserOrEqualValue"/>.
        /// </summary>
        [StackTraceHidden]
        public static void IsGreaterThanOrEqualTo<T>(T lesserOrEqualValue, T actualValue, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>
        {
            Guard.IsGreaterThanOrEqualTo(actualValue, lesserOrEqualValue, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is null or matches the regular expression <paramref name="regularExpression"/>.
        /// </summary>
        [StackTraceHidden]
        public static void MatchesRegExp(string regularExpression, string value, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsRegexMatch(value, regularExpression, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is null or not matches the regular expression <paramref name="regularExpression"/>.
        /// </summary>
        [StackTraceHidden]
        public static void MatchesNotRegExp(string regularExpression, string value, string errorLocation = null, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsNotRegexMatch(value, regularExpression, customMessage, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Call the Validate() method for <paramref name="value"/>
        /// </summary>
        [StackTraceHidden]
        public static void IsValidated(object value, string errorLocation = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            Guard.IsValid(value, null, lineNumber, filePath, memberName);
        }

        /// <summary>
        /// Call the Validate() method for each item in <paramref name="values"/>
        /// </summary>
        [StackTraceHidden]
        public static void IsValidated(IEnumerable<object> values, string errorLocation = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            if (values == null) return;
            foreach (var value in values)
            {
                Guard.IsValid(value, null, lineNumber, filePath, memberName);
            }
        }
    }
}
