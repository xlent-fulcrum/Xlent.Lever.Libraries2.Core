using System;
using System.Collections.Generic;
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
            where TItem : class
    {
        private static readonly string Namespace = typeof(MemoryPersistance<TItem, TId>).Namespace;
        private readonly Dictionary<TId, TItem> _memoryItems = new Dictionary<TId, TItem>();

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
            lock (_memoryItems)
            {
                ValidateNotExists(id);
                MaybeSetId(id, itemCopy);
                _memoryItems.Add(id, itemCopy);
            }
            await Task.Yield();
        }

        /// <inheritdoc />
        public override Task<TItem> ReadAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));

            lock (_memoryItems)
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

            lock (_memoryItems)
            {
                ValidateExists(id);
                SetMemoryItem(id, itemCopy);
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Idempotent, i.e. will not throw an exception if the item does not exist.
        /// </remarks>
        public override Task DeleteAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            lock (_memoryItems)
            {
                if (!_memoryItems.ContainsKey(id)) return Task.FromResult(0);
                _memoryItems.Remove(id);
            }
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public override Task<PageEnvelope<TItem>> ReadAllAsync(int offset = 0, int? limit = null)
        {
            limit = limit ?? PageInfo.DefaultLimit;
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            lock (_memoryItems)
            {
                var keys = _memoryItems.Keys.Skip(offset).Take(limit.Value);
                var list = keys.Select(GetMemoryItem).ToList();
                var page = new PageEnvelope<TItem>(offset, limit.Value, _memoryItems.Count, list);
                return Task.FromResult(page);
            }
        }

        /// <inheritdoc />
        public override Task DeleteAllAsync()
        {
            lock (_memoryItems)
            {
                _memoryItems.Clear();
            }
            return Task.FromResult(0);
        }

        #region private

        private void ValidateNotExists(TId id)
        {
            if (!_memoryItems.ContainsKey(id)) return;
            throw new FulcrumConflictException(
                $"An item of type {typeof(TItem).Name} with id {id} already exists.");
        }

        private void ValidateExists(TId id)
        {
            if (_memoryItems.ContainsKey(id)) return;
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
            _memoryItems[id] = CopyItem(item);
        }

        private TItem GetMemoryItem(TId id)
        {
            ValidateExists(id);
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var item = _memoryItems[id];
            FulcrumAssert.IsNotNull(item, $"{Namespace}: B431A6BB-4A76-4672-9607-65E1C6EBFBC9");
            return CopyItem(item);
        }
        #endregion
    }
}
