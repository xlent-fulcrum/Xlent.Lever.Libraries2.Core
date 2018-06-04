﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xlent.Lever.Libraries2.Core.Misc;

namespace Xlent.Lever.Libraries2.Core.Guards
{
    /// <summary>
    /// A generic class for asserting things that the programmer thinks is true. Generic in the meaning that a parameter says what exception that should be thrown when an assumption is false.
    /// </summary>
    public interface IGuard
    {
        /// <summary>
        /// Will always fail. Used for instance as a default case in a switch statement where all cases should be covered, so we should never end up in the default case.
        /// </summary>
        void Fail(string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is TRUE.
        /// </summary>
        void IsTrue(bool value, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is FALSE.
        /// </summary>
        void IsFalse(bool value, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is NULL.
        /// </summary>
        void IsNull(object value, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is not NULL.
        /// </summary>
        void IsNotNull(object value, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is not the default value for that type.
        /// </summary>
        void IsDefaultValue<T>(T value, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is not the default value for that type.
        /// </summary>
        void IsNotDefaultValue<T>(T value, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is null, empty or only contains white space.
        /// </summary>
        void IsNullOrWhiteSpace(string value, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is not null, not empty and has other characters than white space.
        /// </summary>
        void IsNotNullOrWhiteSpace(string value, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is equal to <paramref name="expectedValue"/>.
        /// </summary>
        void AreEqual(object value, object expectedValue, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is not equal to <paramref name="unexpectedValue"/>.
        /// </summary>
        void AreNotEqual(object value, object unexpectedValue, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is null or implements <paramref name="expectedType"/>.
        /// </summary>
        void IsAssignableTo(Type value, Type expectedType, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is null or not implements <paramref name="unexpectedType"/>.
        /// </summary>
        void IsNotAssignableTo(Type value, Type unexpectedType, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is null or of a type that implements <paramref name="expectedType"/>.
        /// </summary>
        void IsInstanceOf(object value, Type expectedType, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is null or not of a type that implements  <paramref name="unexpectedType"/>.
        /// </summary>
        void IsNotInstanceOf(object value, Type unexpectedType, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is null or less than to <paramref name="greaterValue"/>.
        /// </summary>
        void IsLessThan<T>(T value, T greaterValue, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>;

        /// <summary>
        /// Verify that <paramref name="value"/> is null or less than or equal to <paramref name="greaterOrEqualValue"/>.
        /// </summary>
        void IsLessThanOrEqualTo<T>(T value, T greaterOrEqualValue, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>;

        /// <summary>
        /// Verify that <paramref name="value"/> is null or greater than <paramref name="lesserValue"/>.
        /// </summary>
        void IsGreaterThan<T>(T value, T lesserValue, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>;

        /// <summary>
        /// Verify that <paramref name="value"/> is null or is greater than or equal to <paramref name="lesserOrEqualValue"/>.
        /// </summary>
        void IsGreaterThanOrEqualTo<T>(T value, T lesserOrEqualValue, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
            where T : IComparable<T>;

        /// <summary>
        /// Verify that <paramref name="value"/> is null or matches the regular expression <paramref name="regularExpression"/>.
        /// </summary>
        [StackTraceHidden]
        void IsRegexMatch(string value, string regularExpression, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is null or matches the regular expression <paramref name="regularExpression"/>.
        /// </summary>
        [StackTraceHidden]
        void IsNotRegexMatch(string value, string regularExpression, string customMessage = null,
        [CallerLineNumber] int lineNumber = 0,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is null or that the validation rules are obeyed to.
        /// </summary>
        void IsValid(object value, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="values"/> is null or that all items are <see cref="IsValid(object,string,int,string,string)"/>.
        /// </summary>
        void IsValid(IEnumerable<object> values, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");

        /// <summary>
        /// Verify that <paramref name="value"/> is null or that the validation rules are obeyed to.
        /// </summary>
        void IsNotValid(object value, string customMessage = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "");
    }
}
