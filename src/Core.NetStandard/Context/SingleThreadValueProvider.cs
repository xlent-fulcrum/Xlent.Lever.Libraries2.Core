using System.Collections.Generic;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Context
{
    /// <summary>
    /// Stores values in the execution context which is unaffected by asynchronous code that switches threads or context. 
    /// </summary>
    /// <remarks>Updating values in a thread will not affect the value in parent/sibling threads</remarks>
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
            InternalContract.RequireNotNull(data, nameof(data));
            Dictionary[name] = data;
        }
    }
}
