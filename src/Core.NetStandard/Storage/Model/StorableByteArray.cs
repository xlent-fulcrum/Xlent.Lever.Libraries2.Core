using Newtonsoft.Json;
using Xlent.Lever.Libraries2.Core.Misc.Models;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Contains a byte array
    /// </summary>
    public class StorableByteArray<TId> : IStorableByteArray<TId>, IOptimisticConcurrencyControlByETag, IDeepCopy<IStorableByteArray<TId>>
    {
        /// <inheritdoc />
        public virtual void Validate(string errorLocation, string propertyPath = "")
        {
        }

        /// <inheritdoc />
        public TId Id { get; set; }

        /// <inheritdoc />
        public byte[] ByteArray { get; set; }

        /// <inheritdoc />
        public string ETag { get; set; }

        /// <inheritdoc />
        public virtual IStorableByteArray<TId> DeepCopy()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<StorableByteArray<TId>>(serialized);
        }
    }
}