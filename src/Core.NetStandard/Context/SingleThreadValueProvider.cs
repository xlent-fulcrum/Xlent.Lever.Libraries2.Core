using System.Collections.Concurrent;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Context
{
    /// <summary>
    /// Stores values in a class variable. All references to the same class, same thread or not, will share the same values.
    /// </summary>
    /// <remarks>Provided to use in tests, as many other context </remarks>
    public class SingleThreadValueProvider : IValueProvider
    {
        private static readonly ConcurrentDictionary<string, object> Dictionary = new ConcurrentDictionary<string, object>();

        /// <inheritdoc />
        public T GetValue<T>(string key)
        {
            InternalContract.RequireNotNullOrWhiteSpace(key, nameof(key));
            if (!Dictionary.ContainsKey(key)) return default(T);
            return (T)Dictionary[key];
        }

        /// <inheritdoc />
        public void SetValue<T>(string name, T data)
        {
            InternalContract.RequireNotNullOrWhiteSpace(name, nameof(name));
            Dictionary[name] = data;
        }
    }
}
