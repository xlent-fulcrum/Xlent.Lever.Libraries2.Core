using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// General class for storing any <see cref="IUniquelyIdentifiable{TId}"/> in memory.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class MemoryPersistance<TItem, TId> : CrudBase<TItem, TId>
    {
        private static readonly string Namespace = typeof(MemoryPersistance<TItem, TId>).Namespace;

        /// <summary>
        /// The actual storage of the items.
        /// </summary>
        protected readonly ConcurrentDictionary<TId, TItem> MemoryItems = new ConcurrentDictionary<TId, TItem>();

        /// <inheritdoc />
        public override async Task<TId> CreateAsync(TItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = StorageHelper.CreateNewId<TId>();
            await CreateWithSpecifiedIdAsync(id, item);
            return id;
        }

        /// <inheritdoc />
        public override async Task CreateWithSpecifiedIdAsync(TId id, TItem item)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);

            var itemCopy = CopyItem(item);

            MaybeCreateNewEtag(itemCopy);
            MaybeUpdateTimeStamps(itemCopy, true);
            lock (MemoryItems)
            {
                ValidateNotExists(id);
                MaybeSetId(id, itemCopy);
                var success = MemoryItems.TryAdd(id, itemCopy);
                if (!success) throw new FulcrumConflictException($"Item with id {id} already exists.");
            }
            await Task.Yield();
        }

        /// <inheritdoc />
        public override Task<TItem> ReadAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));

            lock (MemoryItems)
            {
                var itemCopy = GetMemoryItem(id);
                return Task.FromResult(itemCopy);
            }
        }

        /// <inheritdoc />
        public override async Task UpdateAsync(TId id, TItem item)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);

            await MaybeVerifyEtagForUpdateAsync(id, item);
            var itemCopy = CopyItem(item);
            MaybeUpdateTimeStamps(itemCopy, false);
            MaybeCreateNewEtag(itemCopy);

            lock (MemoryItems)
            {
                ValidateExists(id);
                SetMemoryItem(id, itemCopy);
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Idempotent, i.e. will not throw an exception if the item does not exist.
        /// </remarks>
        public override async Task DeleteAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            lock (MemoryItems)
            {
                if (!MemoryItems.ContainsKey(id)) return;
                MemoryItems.TryRemove(id, out var _);
            }

            await Task.Yield();
        }

        /// <inheritdoc />
        public override Task<PageEnvelope<TItem>> ReadAllWithPagingAsync(int offset = 0, int? limit = null)
        {
            limit = limit ?? PageInfo.DefaultLimit;
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            lock (MemoryItems)
            {
                var keys = MemoryItems.Keys.Skip(offset).Take(limit.Value);
                var list = keys.Select(GetMemoryItem).ToList();
                var page = new PageEnvelope<TItem>(offset, limit.Value, MemoryItems.Count, list);
                return Task.FromResult(page);
            }
        }

        /// <inheritdoc />
        public override Task DeleteAllAsync()
        {
            lock (MemoryItems)
            {
                MemoryItems.Clear();
            }
            return Task.FromResult(0);
        }

        #region private

        private void ValidateNotExists(TId id)
        {
            if (!MemoryItems.ContainsKey(id)) return;
            throw new FulcrumConflictException(
                $"An item of type {typeof(TItem).Name} with id {id} already exists.");
        }

        private void ValidateExists(TId id)
        {
            if (MemoryItems.ContainsKey(id)) return;
            throw new FulcrumNotFoundException(
                $"Could not find an item of type {typeof(TItem).Name} with id {id}.");
        }

        private static TItem CopyItem(TItem source)
        {
            InternalContract.RequireNotNull(source, nameof(source));
            var itemCopy = StorageHelper.DeepCopy(source);
            if (itemCopy == null)
                throw new FulcrumAssertionFailedException("Could not copy an item.");
            return itemCopy;
        }

        private void SetMemoryItem(TId id, TItem item)
        {
            MemoryItems[id] = CopyItem(item);
        }

        private TItem GetMemoryItem(TId id)
        {
            ValidateExists(id);
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var item = MemoryItems[id];
            FulcrumAssert.IsNotNull(item, $"{Namespace}: B431A6BB-4A76-4672-9607-65E1C6EBFBC9");
            return CopyItem(item);
        }
        #endregion
    }
}
