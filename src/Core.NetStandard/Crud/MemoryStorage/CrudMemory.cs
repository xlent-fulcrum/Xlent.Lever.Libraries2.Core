using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Helpers;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.MemoryStorage
{
    /// <summary>
    /// General class for storing any <see cref="IUniquelyIdentifiable{TId}"/> in memory.
    /// </summary>
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public class CrudMemory<TModel, TId> : CrudMemory<TModel, TModel, TId>, ICrud<TModel, TId>
    {
    }

    /// <summary>
    /// General class for storing any <see cref="IUniquelyIdentifiable{TId}"/> in memory.
    /// </summary>
    /// <typeparam name="TModelCreate">The type for creating objects in persistant storage.</typeparam>
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public class CrudMemory<TModelCreate, TModel, TId> : RudMemory<TModel, TId>, ICrud<TModelCreate, TModel, TId>
        where TModel : TModelCreate
    {
        /// <inheritdoc />
        public async Task<TId> CreateAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = StorageHelper.CreateNewId<TId>();
            await CreateWithSpecifiedIdAsync(id, item, token);
            return id;
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = await CreateAsync(item, token);
            return await ReadAsync(id, token);
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(TId id, TModelCreate item, CancellationToken token = default(CancellationToken))
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
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            if (!Exists(id)) throw new FulcrumNotFoundException($"Could not find an item with id {id}");
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            await CreateWithSpecifiedIdAsync(id, item, token);
            return await ReadAsync(id, token);
        }

        #region private

        private void ValidateNotExists(TId id)
        {
            if (!Exists(id)) return;
            throw new FulcrumConflictException(
                $"An item of type {typeof(TModel).Name} with id \"{id}\" already exists.");
        }

        private static TModel CopyItem(TModelCreate source)
        {
            InternalContract.RequireNotNull(source, nameof(source));
            var itemCopy = StorageHelper.DeepCopy<TModel, TModelCreate>(source);
            if (itemCopy == null)
                throw new FulcrumAssertionFailedException("Could not copy an item.");
            return itemCopy;
        }
        #endregion
    }
}

