using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Misc.Models;
using Xlent.Lever.Libraries2.Core.Storage.Logic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Contains a byte array
    /// </summary>
    public class StorableByteArray<TId> : IStorableByteArray<TId>, IOptimisticConcurrencyControlByETag, IDeepCopy<StorableByteArray<TId>>
    {
        /// <inheritdoc />
        public TId Id { get; set; }

        /// <inheritdoc />
        public byte[] ByteArray { get; set; }

        /// <inheritdoc />
        public string Etag { get; set; }

        /// <inheritdoc />
        public void DeepCopy(StorableByteArray<TId> source)
        {
            InternalContract.RequireNotNull(source, nameof(source));
            Id = source.Id;
            ByteArray = source.ByteArray;
            Etag = source.Etag;
        }

        /// <inheritdoc />
        public StorableByteArray<TId> DeepCopy()
        {
            return StorageHelper.DeepCopy(this);
        }
    }
}