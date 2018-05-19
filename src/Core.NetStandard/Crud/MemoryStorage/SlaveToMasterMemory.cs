using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Helpers;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.MemoryStorage
{
    /// <summary>
    /// Functionality for persisting objects in groups.
    /// </summary>
    public class SlaveToMasterMemory<TModel, TId> : SlaveToMasterMemory<TModel, TModel, TId>, ISlaveToMasterCrud<TModel, TId>
    {
    }

    /// <summary>
    /// Functionality for persisting objects in groups.
    /// </summary>
    public class SlaveToMasterMemory<TModelCreate, TModel, TId> : CrudMemory<TModelCreate, TModel, SlaveToMasterId<TId>>, ISlaveToMasterCrud<TModelCreate, TModel, TId>
        where TModel : TModelCreate
    {
        /// <summary>
        /// The storages; One dictionary with a memory storage for each master id.
        /// </summary>
        protected static readonly ConcurrentDictionary<TId, CrudMemory<TModelCreate, TModel, TId>> Storages = new ConcurrentDictionary<TId, CrudMemory<TModelCreate, TModel, TId>>();


        /// <inheritdoc />
        public async Task<SlaveToMasterId<TId>> CreateAsync(TId masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var slaveId = StorageHelper.CreateNewId<TId>();
            var id = new SlaveToMasterId<TId>(masterId, slaveId);
            await CreateWithSpecifiedIdAsync(id, item, token);
            return id;
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TId masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var slaveId = StorageHelper.CreateNewId<TId>();
            var id = new SlaveToMasterId<TId>(masterId, slaveId);
            return await CreateWithSpecifiedIdAndReturnAsync(id, item, token);
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(TId parentId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            if (limit != null)
            {
                InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            }
            var groupPersistance = GetStorage(parentId);
            return await groupPersistance.ReadAllWithPagingAsync(offset, limit, token);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> ReadChildrenAsync(TId masterId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            var groupPersistance = GetStorage(masterId);
            return await groupPersistance.ReadAllAsync(limit, token);
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(TId masterId, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            var groupPersistance = GetStorage(masterId);
            await groupPersistance.DeleteAllAsync(token);
        }

        #region private
        /// <summary>
        ///  Get the storage for a specific <paramref name="masterId"/>.
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        private CrudMemory<TModelCreate, TModel, TId> GetStorage(TId masterId)
        {
            if (!Storages.ContainsKey(masterId)) Storages[masterId] = new CrudMemory<TModelCreate, TModel, TId>();
            return Storages[masterId];
        }
        #endregion
    }
}