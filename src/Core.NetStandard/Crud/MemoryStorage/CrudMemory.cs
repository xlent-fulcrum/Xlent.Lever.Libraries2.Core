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
    /// <typeparam name="TModelCreate">The type for creating objects in persistant storage.</typeparam>
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public class CrudMemory<TModelCreate, TModel, TId> : CrudBase<TModelCreate, TModel, TId>
    where TModel : TModelCreate
    {
        /// <summary>
        /// The actual storage of the items.
        /// </summary>
        protected readonly ConcurrentDictionary<TId, TModel> MemoryItems = new ConcurrentDictionary<TId, TModel>();

        /// <inheritdoc />
        public override async Task<TId> CreateAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = StorageHelper.CreateNewId<TId>();
            await CreateWithSpecifiedIdAsync(id, item, token);
            return id;
        }

        /// <inheritdoc />
        public override async Task CreateWithSpecifiedIdAsync(TId id, TModelCreate item, CancellationToken token = default(CancellationToken))
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
        public override Task<TModel> ReadAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));

            lock (MemoryItems)
            {
                var itemCopy = GetMemoryItem(id, false);
                return Task.FromResult(itemCopy);
            }
        }

        /// <inheritdoc />
        public override async Task UpdateAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);

            await MaybeVerifyEtagForUpdateAsync(id, item, token);
            var itemCopy = CopyItem(item);
            MaybeUpdateTimeStamps(itemCopy, false);
            MaybeCreateNewEtag(itemCopy);

            lock (MemoryItems)
            {
                SetMemoryItem(id, itemCopy);
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Idempotent, i.e. will not throw an exception if the item does not exist.
        /// </remarks>
        public override async Task DeleteAsync(TId id, CancellationToken token = default(CancellationToken))
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
            lock (MemoryItems)
            {
                MemoryItems.Clear();
            }
            return Task.FromResult(0);
        }

        #region private

        private void ValidateNotExists(TId id)
        {
            if (!Exists(id)) return;
            throw new FulcrumConflictException(
                $"An item of type {typeof(TModel).Name} with id \"{id}\" already exists.");
        }

        private bool Exists(TId id)
        {
            return (MemoryItems.ContainsKey(id));
        }

        private static TModel CopyItem(TModel source)
        {
            InternalContract.RequireNotNull(source, nameof(source));
            var itemCopy = StorageHelper.DeepCopy(source);
            if (itemCopy == null)
                throw new FulcrumAssertionFailedException("Could not copy an item.");
            return itemCopy;
        }

        private static TModel CopyItem(TModelCreate source)
        {
            InternalContract.RequireNotNull(source, nameof(source));
            var itemCopy = StorageHelper.DeepCopy<TModel, TModelCreate>(source);
            if (itemCopy == null)
                throw new FulcrumAssertionFailedException("Could not copy an item.");
            return itemCopy;
        }

        private void SetMemoryItem(TId id, TModel item)
        {
            MemoryItems[id] = CopyItem(item);
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
