using System;
using Newtonsoft.Json;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// Helper methods for Storage
    /// </summary>
    public static class StorageHelper
    {
        /// <summary>
        /// Create a new Id of type <see cref="string"/> or type <see cref="Guid"/>.
        /// </summary>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public static TId CreateNewId<TId>()
        {
            var id = default(TId);
            if (typeof(TId) == typeof(Guid))
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                id = (dynamic)Guid.NewGuid();
            }
            else if (typeof(TId) == typeof(string))
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                id = (dynamic)Guid.NewGuid().ToString();
            }
            else
            {
                FulcrumAssert.Fail(null,
                    $"{nameof(CreateNewId)} can handle Guid and string as type for Id, but it can't handle {typeof(TId)}.");
            }
            return id;
        }

        /// <summary>
        /// A generic method for deep copying.
        /// </summary>
        /// <param name="source">The object that should be copied.</param>
        /// <returns>A copied object.</returns>
        public static T DeepCopy<T>(T source)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }
    }
}
