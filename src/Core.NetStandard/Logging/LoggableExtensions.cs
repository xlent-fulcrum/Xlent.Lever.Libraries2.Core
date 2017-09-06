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
        /// <inheritdoc cref="ILoggable.ToLogString"/>
        public static string ToLogString(this DateTimeOffset value) =>
            value.ToString("o", CultureInfo.InvariantCulture);

        /// <inheritdoc cref="ILoggable.ToLogString"/>
        public static string ToLogString(this JToken value) =>
            value.ToString(Formatting.Indented);

        /// <inheritdoc cref="ILoggable.ToLogString"/>
        public static string ToLogString(this JObject value) =>
            value.ToString(Formatting.Indented);

        /// <inheritdoc cref="ILoggable.ToLogString"/>
        public static string ToLogString(this JValue value) =>
            value.ToString(Formatting.Indented);

        /// <inheritdoc cref="ILoggable.ToLogString"/>
        public static string ToLogString(this JArray values)
        {
            var allStrings = values.Select(v => v.ToLogString());
            var oneString = string.Join(", ", allStrings);
            return $"({oneString})";
        }

        /// <inheritdoc cref="ILoggable.ToLogString"/>
        public static string ToLogString<T>(this IEnumerable<T> values)
            where T : ILoggable
        {
            var allStrings = values.Select(v => v.ToLogString());
            var oneString = string.Join(", ", allStrings);
            return $"({oneString})";
        }

        /// <inheritdoc cref="ILoggable.ToLogString"/>
        public static string ToLogString(this Exception value)
        {
            if (value == null) return "";
            try
            {
                var formatted = $"value type: {value.GetType().FullName}";
                var fulcrumvalue = value as FulcrumException;
                if (fulcrumvalue != null) formatted += $"\r{fulcrumvalue.ToLogString()}";
                formatted += $"\rvalue message: {value.Message}";
                formatted += $"\r{value.StackTrace}";
                formatted += AddInnerExceptions(value);
                return formatted;
            }
            catch (Exception)
            {
                return value.Message;
            }
        }

        private static string AddInnerExceptions(Exception exception)
        {
            var formatted = "";
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                formatted += "\rAggregated exceptions:";
                formatted = aggregateException
                    .Flatten()
                    .InnerExceptions
                    .Aggregate(formatted, (current, innerException) => current + $"\r{innerException.ToLogString()}");
            }
            if (exception.InnerException != null)
            {
                formatted += $"\r--Inner exception--\r{exception.InnerException.ToLogString()}";
            }
            return formatted;
        }
    }
}
