using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Assert
{
    /// <summary>
    /// A class for verifying method contracts. Will throw <see cref="FulcrumContractException"/> if the contract is broken.
    /// </summary>
    public static class InternalContract
    {
        private static readonly string Namespace = typeof(InternalContract).Namespace;
        /// <summary>
        /// Verify that <paramref name="expression"/> return true, when applied to <paramref name="parameterValue"/>.
        /// </summary>
        public static void Require<TParameter>(TParameter parameterValue,
            Expression<Func<TParameter, bool>> expression, string parameterName)
        {
            GenericContract<FulcrumContractException>.Require(parameterValue, expression, parameterName);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is not null.
        /// </summary>
        public static void RequireNotNull(object parameterValue, string parameterName, string customMessage = null)
        {
            GenericContract<FulcrumContractException>.RequireNotNull(parameterValue, parameterName, customMessage);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is not the default parameterValue for this type.
        /// </summary>
        public static void RequireNotDefaultValue<TParameter>(TParameter parameterValue, string parameterName, string customMessage = null)
        {
            GenericContract<FulcrumContractException>.RequireNotDefaultValue(parameterValue, parameterName, customMessage);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is not null, not empty and contains other characters than white space.
        /// </summary>
        public static void RequireNotNullOrWhitespace(string parameterValue, string parameterName, string customMessage = null)
        {
            GenericContract<FulcrumContractException>.RequireNotNullOrWhitespace(parameterValue, parameterName, customMessage);
        }

        /// <summary>
        /// If <paramref name="parameterValue"/> is not null, then call the FulcrumValidate() method of that type.
        /// </summary>
        [Obsolete("Use the RequireValidated() method.")]
        public static void RequireValidatedOrNull(IValidatable parameterValue, string parameterName, string customMessage = null)
        {
            if (parameterValue == null) return;
            try
            {
                parameterValue.Validate($"{Namespace}: F8A9DE78-28E2-4FC1-A1D7-88020E720525");
            }
            catch (FulcrumContractException e)
            {
                throw new FulcrumContractException($"Validation failed for {parameterName}: {e.Message}", e);
            }
        }

        /// <summary>
        /// If <paramref name="parameterValues"/> is not null, then call the FulcrumValidate() method of that type.
        /// </summary>
        [Obsolete("Use the RequireValidated() method.")]
        public static void RequireValidatedOrNull(IEnumerable<IValidatable> parameterValues, string parameterName, string customMessage = null)
        {
            if (parameterValues == null) return;
            foreach (var parameterValue in parameterValues)
            {
                RequireValidatedOrNull(parameterValue, parameterName, customMessage);
            }
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is not null and also call the FulcrumValidate() method of that type.
        /// </summary>
        [Obsolete("Use the RequireValidated() method.")]
        public static void RequireValidatedAndNotNull(IValidatable parameterValue, string parameterName, string customMessage = null)
        {
            RequireNotNull(parameterValue, parameterName);
            RequireValidatedOrNull(parameterValue, parameterName, customMessage);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValues"/> is not null and also call the FulcrumValidate() method of that type.
        /// </summary>
        [Obsolete("Use the RequireValidated() method.")]
        public static void RequireValidatedAndNotNull(IEnumerable<IValidatable> parameterValues, string parameterName, string customMessage = null)
        {
            RequireNotNull(parameterValues, parameterName);
            RequireValidatedOrNull(parameterValues, parameterName, customMessage);
        }

        /// <summary>
        /// If <paramref name="parameterValue"/> is not null, then call the Validate() method of that type.
        /// </summary>
        public static void RequireValidated(IValidatable parameterValue, string parameterName, string customMessage = null)
        {
            if (parameterValue == null) return;
            try
            {
                parameterValue.Validate($"{Namespace}: F8A9DE78-28E2-4FC1-A1D7-88020E720525");
            }
            catch (FulcrumAssertionFailedException e)
            {
                throw new FulcrumContractException(customMessage ?? $"Validation failed for {parameterName}: {e.Message}", e);
            }
        }

        /// <summary>
        /// If <paramref name="parameterValues"/> is not null, then call the Validate() method for each item.
        /// </summary>
        public static void RequireValidated(IEnumerable<IValidatable> parameterValues, string parameterName, string customMessage = null)
        {
            if (parameterValues == null) return;
            foreach (var parameterValue in parameterValues)
            {
                RequireValidated(parameterValue, parameterName, customMessage);
            }
        }

        /// <summary>
        /// Verify that <paramref name="expression"/> returns a true parameterValue.
        /// </summary>
        [Obsolete("Please notify the Fulcrum team if you use this assertion method. We intend to remove it.")]
        public static void Require(Expression<Func<bool>> expression, string message)
        {
            InternalContract.RequireNotNullOrWhitespace(message, nameof(message));
            GenericContract<FulcrumContractException>.Require(expression, message);
        }

        /// <summary>
        /// Verify that <paramref name="mustBeTrue"/> really is true.
        /// </summary>
        public static void Require(bool mustBeTrue, string message)
        {
            InternalContract.RequireNotNullOrWhitespace(message, nameof(message));
            GenericContract<FulcrumContractException>.Require(mustBeTrue, message);
        }

        /// <summary>
        /// Verify that <paramref name="parameterValue"/> is less than to <paramref name="greaterValue"/>.
        /// </summary>
        public static void RequireLessThan<T>(T greaterValue, T parameterValue, string parameterName, string customMessage = null)
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
        public static void RequireLessThanOrEqualTo<T>(T greaterOrEqualValue, T parameterValue, string parameterName, string customMessage = null)
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
        public static void RequireGreaterThan<T>(T lesserValue, T parameterValue, string parameterName, string customMessage = null)
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
        public static void RequireGreaterThanOrEqualTo<T>(T lesserOrEqualValue, T parameterValue, string parameterName, string customMessage = null)
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
        public static void MatchesRegExp(string regularExpression, string parameterValue, string parameterName, string customMessage = null)
        {
            RequireNotNullOrWhitespace(regularExpression, nameof(regularExpression));
            RequireNotNull(parameterName, nameof(parameterName));
            GenericContract<FulcrumContractException>.RequireMatchesRegExp(regularExpression, parameterValue, parameterName, customMessage);
        }

        /// <summary>
        /// Verify that <paramref name="value"/> is null or not matches the regular expression <paramref name="regularExpression"/>.
        /// </summary>
        public static void MatchesNotRegExp(string regularExpression, string value, string errorLocation, string customMessage = null)
        {
            RequireNotNullOrWhitespace(regularExpression, nameof(regularExpression));
            RequireNotNull(errorLocation, nameof(errorLocation));
            GenericContract<FulcrumContractException>.RequireMatchesNotRegExp(regularExpression, value, errorLocation, customMessage);
        }

        /// <summary>
        /// Always fail, with the given <paramref name="message"/>.
        /// </summary>
        public static void Fail(string message)
        {
            RequireNotNull(message, nameof(message));
            GenericContract<FulcrumContractException>.Fail(message);
        }
    }
}
