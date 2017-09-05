using System.Collections.Generic;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Context
{
    /// <summary>
    /// Stores values in a class variable. All references to the same class, same thread or not, will share the same values.
    /// </summary>
    /// <remarks>Provided to use in tests, as many other context </remarks>
    public class SingleThreadValueProvider : IValueProvider
    {
        private static readonly Dictionary<string, object> Dictionary = new Dictionary<string, object>();

        /// <inheritdoc />
        public T GetValue<T>(string key)
        {
            InternalContract.RequireNotNullOrWhitespace(key, nameof(key));
            if (!Dictionary.ContainsKey(key)) return default(T);
            return (T)Dictionary[key];
        }

        /// <inheritdoc />
        public void SetValue<T>(string name, T data)
        {
            InternalContract.RequireNotNullOrWhitespace(name, nameof(name));
            Dictionary[name] = data;
        }
    }
}
