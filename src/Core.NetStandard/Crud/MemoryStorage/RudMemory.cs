using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Helpers;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.MemoryStorage
{
    /// <summary>
    /// General class for storing any <see cref="IUniquelyIdentifiable{TId}"/> in memory.
    /// </summary>
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public class RudMemory<TModel, TId> : RudBase<TModel, TId>
    {
        /// <summary>
        /// The actual storage of the items.
        /// </summary>
        protected readonly ConcurrentDictionary<TId, TModel> MemoryItems = new ConcurrentDictionary<TId, TModel>();

        /// <inheritdoc />
        public override Task<TModel> ReadAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));

            var itemCopy = GetMemoryItem(id, false);
            return Task.FromResult(itemCopy);
        }

        /// <inheritdoc />
        public override async Task UpdateAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            if (!Exists(id)) throw new FulcrumNotFoundException($"Update failed. Could not find an item with id {id}.");

            var oldValue = await MaybeVerifyEtagForUpdateAsync(id, item, token);
            var itemCopy = CopyItem(item);
            MaybeUpdateTimeStamps(itemCopy, false);
            MaybeCreateNewEtag(itemCopy);

            MemoryItems[id] = itemCopy;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Idempotent, i.e. will not throw an exception if the item does not exist.
        /// </remarks>
        public override async Task DeleteAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));

            MemoryItems.TryRemove(id, out var _);

            await Task.Yield();
        }

        /// <inheritdoc />
        public override Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            limit = limit ?? PageInfo.DefaultLimit;
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            lock (MemoryItems)
            {
                var keys = MemoryItems.Keys.Skip(offset).Take(limit.Value);
                var list = keys.Select(id => GetMemoryItem(id, false)).ToList();
                var page = new PageEnvelope<TModel>(offset, limit.Value, MemoryItems.Count, list);
                return Task.FromResult(page);
            }
        }

        /// <inheritdoc />
        public override Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            MemoryItems.Clear();
            return Task.FromResult(0);
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IOptimisticConcurrencyControlByETag"/>
        /// then the old value is read using <see cref="ReadBase{TModel,TId}.ReadAsync"/> and the values are verified to be equal.
        /// The Etag of the item is then set to a new value.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <returns></returns>
        protected virtual async Task<TModel> MaybeVerifyEtagForUpdateAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            var oldItem = await ReadAsync(id, token);
            if (!(item is IOptimisticConcurrencyControlByETag etaggable)) return oldItem;
            if (Equals(oldItem, default(TModel))) return oldItem;
            var oldEtag = (oldItem as IOptimisticConcurrencyControlByETag)?.Etag;
            if (oldEtag?.ToLowerInvariant() != etaggable.Etag?.ToLowerInvariant())
                throw new FulcrumConflictException($"The updated item ({item}) had an old ETag value.");

            return oldItem;
        }

        #region private
        /// <summary>
        /// Return true if an item iwht id <paramref name="id"/> exists
        /// </summary>
        protected bool Exists(TId id)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return (MemoryItems.ContainsKey(id));
        }

        /// <summary>
        /// Copy an item into a new instance.
        /// </summary>
        /// <exception cref="FulcrumAssertionFailedException"></exception>
        protected static TModel CopyItem(TModel source)
        {
            InternalContract.RequireNotNull(source, nameof(source));
            var itemCopy = StorageHelper.DeepCopy(source);
            if (itemCopy == null)
                throw new FulcrumAssertionFailedException("Could not copy an item.");
            return itemCopy;
        }

        private TModel GetMemoryItem(TId id, bool okIfNotExists)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            if (!Exists(id))
            {
                if (!okIfNotExists)
                    throw new FulcrumNotFoundException(
                        $"Could not find an item of type {typeof(TModel).Name} with id \"{id}\".");
                return default(TModel);
            }
            var item = MemoryItems[id];
            FulcrumAssert.IsNotNull(item);
            return CopyItem(item);
        }
        #endregion
    }
}
