using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Misc.Models;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// General class for storing any <see cref="IStorableItem{TId}"/> in memory.
    /// </summary>
    /// <typeparam name="TStorableItem"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class MemoryPersistance<TStorableItem, TId> : ICrudAll<TStorableItem, TId>
            where TStorableItem : class, IStorableItem<TId>, IOptimisticConcurrencyControlByETag, IDeepCopy<TStorableItem>, IValidatable
    {
        private static readonly string Namespace = typeof(MemoryPersistance<TStorableItem, TId>).Namespace;
        // ReSharper disable once StaticMemberInGenericType
        private static int _nextInteger = 1;
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object LockObject = new object();
        private readonly Dictionary<TId, TStorableItem> _memoryItems = new Dictionary<TId, TStorableItem>();

        /// <inheritdoc />
        public Task<TStorableItem> CreateAsync(TStorableItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            InternalContract.RequireValidated(item, nameof(item));

            TId id = default(TId);
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
            else if (typeof(TId) == typeof(int))
            {
                lock (LockObject)
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    id = (dynamic)_nextInteger++;
                }
            }
            else
            {
                FulcrumAssert.Fail($"{Namespace}: 5CBE07D8-4C31-43E7-A41C-1DF0B173ABF9", $"MemoryStorage can handle Guid, string and int as type for Id, but it can't handle {typeof(TId)}.");
            }
            return CreateWithSpecifiedIdAsync(id, item);
        }

        /// <summary>
        /// Obsolete.
        /// </summary>
        /// <exception cref="FulcrumNotImplementedException"></exception>
        [Obsolete("Method has been renamed to CreateWithSpecifiedIdAsync.", true)]
        public Task<TStorableItem> CreateAsync(TId id, TStorableItem item)
        {
            throw new FulcrumNotImplementedException();
        }

        /// <summary>
        /// Same as <see cref="CreateAsync(TStorableItem)"/>, but you can specify the new id.
        /// </summary>
        /// <param name="id">The id to use for the new item.</param>
        /// <param name="item">The item to create in storage.</param>
        /// <returns>The newly created item.</returns>
        public Task<TStorableItem> CreateWithSpecifiedIdAsync(TId id, TStorableItem item)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotNull(item, nameof(item));
            InternalContract.RequireValidated(item, nameof(item));

            var itemCopy = CopyItem(item);
            itemCopy.ETag = Guid.NewGuid().ToString();
            itemCopy.Id = id;
            lock (_memoryItems)
            {
                ValidateNotExists(itemCopy.Id);
                _memoryItems.Add(itemCopy.Id, itemCopy);
                return Task.FromResult(GetMemoryItem(itemCopy.Id));
            }
        }

        /// <inheritdoc />
        public Task<TStorableItem> ReadAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));

            lock (_memoryItems)
            {
                var itemCopy = GetMemoryItem(id);
                return Task.FromResult(itemCopy);
            }
        }

        /// <inheritdoc />
        public Task<TStorableItem> UpdateAsync(TStorableItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            InternalContract.RequireValidated(item, nameof(item));
            InternalContract.RequireNotDefaultValue(item.Id, nameof(item.Id));

            var itemCopy = CopyItem(item);
            lock (_memoryItems)
            {
                ValidateExists(itemCopy.Id);
                var current = GetMemoryItem(itemCopy.Id);
                ValidateEtag(current, itemCopy);
                itemCopy.ETag = Guid.NewGuid().ToString();
                SetMemoryItem(itemCopy);
                return Task.FromResult(GetMemoryItem(itemCopy.Id));
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Idempotent, i.e. will not throw an exception if the item does not exist.
        /// </remarks>
        public Task DeleteAsync(TId id)
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
        public Task<PageEnvelope<TStorableItem, TId>> ReadAllAsync(int offset = 0, int? limit = null)
        {
            limit = limit ?? PageInfo.DefaultLimit;
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            lock (_memoryItems)
            {
                var keys = _memoryItems.Keys.Skip(offset).Take(limit.Value);
                var list = keys.Select(GetMemoryItem).ToList();
                var page = new PageEnvelope<TStorableItem, TId>(offset, limit.Value, _memoryItems.Count, list);
                return Task.FromResult(page);
            }
        }

        /// <inheritdoc />
        public Task DeleteAllAsync()
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
                $"An item of type {typeof(TStorableItem).Name} with id {id} already exists.");
        }

        private void ValidateExists(TId id)
        {
            if (_memoryItems.ContainsKey(id)) return;
            throw new FulcrumNotFoundException(
                $"Could not find an item of type {typeof(TStorableItem).Name} with id {id}.");
        }

        private void ValidateEtag(IStorableItem<TId> current, IStorableItem<TId> item)
        {
            var currentWithEtag = current as IOptimisticConcurrencyControlByETag;
            var itemWithEtag = item as IOptimisticConcurrencyControlByETag;
            if (currentWithEtag == null || itemWithEtag == null) return;

            if (string.Equals(currentWithEtag.ETag, itemWithEtag.ETag, StringComparison.OrdinalIgnoreCase)) return;
            throw new FulcrumConflictException(
                $"The item of type {typeof(TStorableItem).Name} with id {item.Id} has been updated by someone else. Please get a fresh copy and try again.");
        }

        private static TStorableItem CopyItem(TStorableItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            var itemCopy = item?.DeepCopy();
            if (itemCopy == null)
                throw new FulcrumAssertionFailedException("Could not copy an item.",
                    $"{Namespace}: F517B23A-CB23-4B69-A3AE-7F52CD804352");
            return itemCopy;
        }

        private void SetMemoryItem(TStorableItem item)
        {
            _memoryItems[item.Id] = CopyItem(item);
        }

        private TStorableItem GetMemoryItem(TId id)
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
