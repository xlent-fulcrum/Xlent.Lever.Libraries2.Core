using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Logging
{
    /// <summary>
    /// Extensions to some non-fulcrum classes to make them implement the methods in ILoggable.
    /// </summary>
    public static class LoggableExtensions
    {
        /// <summary>
        /// Very much like <see cref="object.ToString"/>, but specifically for logging purposes.
        /// </summary>
        /// <returns>A string for logging information about this type of object.</returns>
        public static string ToLogString(this DateTimeOffset value) =>
            value.ToString("o", CultureInfo.InvariantCulture);

        /// <summary>
        /// Very much like <see cref="object.ToString"/>, but specifically for logging purposes.
        /// </summary>
        /// <returns>A string for logging information about this type of object.</returns>
        public static string ToLogString(this JToken value) =>
            value.ToString(Formatting.Indented);

        /// <summary>
        /// Very much like <see cref="object.ToString"/>, but specifically for logging purposes.
        /// </summary>
        /// <returns>A string for logging information about this type of object.</returns>
        public static string ToLogString(this JObject value) =>
            value.ToString(Formatting.Indented);

        /// <summary>
        /// Very much like <see cref="object.ToString"/>, but specifically for logging purposes.
        /// </summary>
        /// <returns>A string for logging information about this type of object.</returns>
        public static string ToLogString(this JValue value) =>
            value.ToString(Formatting.Indented);

        /// <summary>
        /// Very much like <see cref="object.ToString"/>, but specifically for logging purposes.
        /// </summary>
        /// <returns>A string for logging information about this type of object.</returns>
        public static string ToLogString(this JArray values)
        {
            var allStrings = values.Select(v => v.ToLogString());
            var oneString = string.Join(", ", allStrings);
            return $"({oneString})";
        }

        /// <summary>
        /// Very much like <see cref="object.ToString"/>, but specifically for logging purposes.
        /// </summary>
        /// <returns>A string for logging information about this type of object.</returns>
        public static string ToLogString<T>(this IEnumerable<T> values)
            where T : ILoggable
        {
            var allStrings = values.Select(v => v.ToLogString());
            var oneString = string.Join(", ", allStrings);
            return $"({oneString})";
        }

        /// <summary>
        /// Very much like <see cref="object.ToString"/>, but specifically for logging purposes.
        /// </summary>
        /// <returns>A string for logging information about this type of object.</returns>
        public static string ToLogString(this Exception value)
        {
            return value.ToLogString(false);
        }

        /// <summary>
        /// Very much like <see cref="object.ToString"/>, but specifically for logging purposes.
        /// </summary>
        /// <param name="value">The exception to base the string on.</param>
        /// <param name="hideStackTrace">When this is true, any stack trace will be hidden.</param>
        /// <returns>A string for logging information about this type of object.</returns>
        public static string ToLogString(this Exception value, bool hideStackTrace)
        {
            if (value == null) return "";
            try
            {
                var formatted = $"{value.GetType().FullName} {value.Message}";
                var fulcrumvalue = value as FulcrumException;
                if (fulcrumvalue != null) formatted += $"\r{fulcrumvalue.ToLogString()}";
                if (!hideStackTrace) formatted += $"\r{value.StackTrace}";
                formatted += AddInnerExceptions(value, hideStackTrace);
                return formatted;
            }
            catch (Exception)
            {
                return value.Message;
            }
        }

        private static string AddInnerExceptions(Exception exception, bool hideStackTrace)
        {
            var formatted = "";
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                formatted += "\rAggregated exceptions:";
                formatted = aggregateException
                    .Flatten()
                    .InnerExceptions
                    .Aggregate(formatted, (current, innerException) => current + $"\r{innerException.ToLogString(hideStackTrace)}");
            }
            if (exception.InnerException != null)
            {
                formatted += $"\r--Inner exception--\r{exception.InnerException.ToLogString(hideStackTrace)}";
            }
            return formatted;
        }
    }
}
