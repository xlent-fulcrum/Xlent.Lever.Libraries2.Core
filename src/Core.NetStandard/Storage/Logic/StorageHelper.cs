using System;
using Newtonsoft.Json;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

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

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IValidatable"/>, then it is validated.
        /// </summary>
        public static void MaybeValidate<TItem>(TItem item)
        {
            if (item is IValidatable validatable) InternalContract.RequireValidated(validatable, nameof(item));
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IOptimisticConcurrencyControlByETag"/>
        /// then the Etag of the item is set to a new value.
        /// </summary>
        public static void MaybeCreateNewEtag<TItem>(TItem item)
        {
            if (item is IOptimisticConcurrencyControlByETag eTaggable) eTaggable.Etag = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IUniquelyIdentifiable{TId}"/>
        /// then the Id of the item is set.
        /// </summary>
        public static void MaybeSetId<TId, TItem>(TId id, TItem item)
        {
            if (item is IUniquelyIdentifiable<TId> identifiable) identifiable.Id = id;
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="ITimeStamped"/>
        /// then the <see cref="ITimeStamped.RecordUpdatedAt"/> is set. If <paramref name="updateCreatedToo"/> is true, 
        /// then the <see cref="ITimeStamped.RecordCreatedAt"/> is also set.
        /// </summary>
        /// <param name="item">The item that will be affected.</param>
        /// <param name="updateCreatedToo">True means that we should update the create property too.</param>
        /// <param name="timeStamp">Optional time stamp to use when setting the time properties. If null, then 
        /// <see cref="DateTimeOffset.Now"/> will be used.</param>
        public static void MaybeUpdateTimeStamps<TItem>(TItem item, bool updateCreatedToo, DateTimeOffset? timeStamp = null)
        {
            if (!(item is ITimeStamped timeStamped)) return;
            timeStamp = timeStamp ?? DateTimeOffset.Now;
            timeStamped.RecordUpdatedAt = timeStamp.Value;
            if (updateCreatedToo) timeStamped.RecordCreatedAt = timeStamp.Value;
        }

        /// <summary>
        /// Helper method to convert from one parameter type to another.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget ConvertBetweenParameterTypes<TTarget, TSource>(TSource source)
        {
            object referenceIdAsObject = source;
            try
            {
                var target = (TTarget)referenceIdAsObject;
                return target;
            }
            catch (Exception e)
            {
                InternalContract.Fail(
                    $"The value \"{source}\" of type {source.GetType().Name} can't be converted into type {source.GetType().Name}:\r" +
                    $"{e.Message}");
                // We should not end up at this line, but the compiler think that we can, so we add a throw here.
                throw;
            }
        }
    }
}
