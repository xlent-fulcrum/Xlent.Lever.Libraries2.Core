using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Cache
{

    /// <summary>
    /// A <see cref="UseCacheStrategyDelegateAsync{TId}"/> should return one of these values.
    /// </summary>
    public enum UseCacheStrategyEnum
    {
        /// <summary>
        /// Use the cached value
        /// </summary>
        Use,
        /// <summary>
        /// Ignore the cached value, but keep it in the cache
        /// </summary>
        Ignore,
        /// <summary>
        /// Ignore the cached value, and remove it from the cache
        /// </summary>
        Remove
    }

    /// <summary>
    /// A delegate for flushing the cache, ie remove all items in the cache.
    /// </summary>
    public delegate Task FlushCacheDelegateAsync(CancellationToken token = default(CancellationToken));

    /// <summary>
    /// The delegate should decide if we should even should try to get the data from the cache, or if we should go directly to the storage.
    /// </summary>
    public delegate Task<bool> UseCacheAtAllDelegateAsync(Type cachedItemType);

    /// <summary>
    /// The delegate will receive information about a cached item. Based on that information, the delegate should decide if the cached data should be deleted, ignored or used.
    /// Before the delegate is called, the cached item has already been vetted according to the <see cref="AutoCacheOptions"/>.
    /// This means for example that you will not have to check if the data is too old in the general sense.
    /// </summary>
    /// <param name="cachedItemInformation">Information about the cached item.</param>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    public delegate Task<UseCacheStrategyEnum> UseCacheStrategyDelegateAsync<TId>(CachedItemInformation<TId> cachedItemInformation, CancellationToken token = default(CancellationToken));

    /// <summary>
    /// A delegate for getting a unique cache key from an item.
    /// </summary>
    /// <param name="item">The item to get the key for</param>
    public delegate TId GetIdDelegate<in TModel, out TId>(TModel item);

    /// <summary>
    /// Methods that are used for serializing.
    /// </summary>
    public static class SerializingSupport
    {

        /// <summary>
        /// Serialize an item into a byte array,
        /// </summary>
        public static byte[] Serialize<T>(T item)
        {
            var itemAsJsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(itemAsJsonString);
        }

        /// <summary>
        /// Deserialize an item from a byte array,
        /// </summary>

        public static T Deserialize<T>(byte[] itemAsBytes)
        {
            var itemAsJsonString = Encoding.UTF8.GetString(itemAsBytes);
            return JsonConvert.DeserializeObject<T>(itemAsJsonString);
        }

        /// <summary>
        ///Deserialize the <paramref name="serializedEnvelope"/> and deserialize the data in it.
        /// </summary>
        public static T ToItem<T>(byte[] serializedEnvelope)
        {
            var cacheEnvelope = Deserialize<CacheEnvelope>(serializedEnvelope);
            return Deserialize<T>(cacheEnvelope.Data);
        }
    }
}
