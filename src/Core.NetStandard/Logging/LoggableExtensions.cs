using System;
using System.Globalization;
using System.Linq;
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
        /// <remarks>Typically contains more detailed information than the normal ToString(). </remarks>
        public static string ToLogString(this DateTimeOffset value) =>
            value.ToString("o", CultureInfo.InvariantCulture);

        /// <summary>
        /// Very much like <see cref="object.ToString"/>, but specifically for logging purposes.
        /// </summary>
        /// <returns>A string for logging information about this type of object.</returns>
        /// <remarks>Typically contains more information than the normal ToString(). </remarks>
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
