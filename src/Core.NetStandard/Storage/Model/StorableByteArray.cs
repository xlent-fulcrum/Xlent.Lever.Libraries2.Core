using System;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Logic;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Contains a byte array
    /// </summary>
    public class StorableByteArray<TId> : IStorableByteArray<TId>, IOptimisticConcurrencyControlByETag
    {
        /// <inheritdoc />
        public TId Id { get; set; }

        /// <inheritdoc />
        public byte[] ByteArray { get; set; }

        /// <inheritdoc />
        public string Etag { get; set; }

        /// <summary>
        /// Copy another object of the same type.
        /// </summary>
        /// <param name="source"></param>
        [Obsolete("Use StorageHelper.DeepCopy", true)]
        public void DeepCopy(StorableByteArray<TId> source)
        {
            InternalContract.RequireNotNull(source, nameof(source));
            Id = source.Id;
            ByteArray = source.ByteArray;
            Etag = source.Etag;
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        [Obsolete("Use StorageHelper.DeepCopy", true)]
        public StorableByteArray<TId> DeepCopy()
        {
            return StorageHelper.DeepCopy(this);
        }
    }
}