using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Helpers;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.MemoryStorage
{
    /// <summary>
    /// Functionality for persisting objects in groups.
    /// </summary>
    public class SlaveToMasterMemory<TModelCreate, TModel, TId> : SlaveToMasterRelationBase<TModelCreate, TModel, TId> where TModel : TModelCreate
    {
        /// <summary>
        /// The storages; One dictionary with a memory storage for each master id.
        /// </summary>
        protected static readonly ConcurrentDictionary<TId, CrudMemory<TModelCreate, TModel, TId>> Storages = new ConcurrentDictionary<TId, CrudMemory<TModelCreate, TModel, TId>>();

        /// <inheritdoc />
        public override async Task<TId> CreateAsync(TId masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(masterId);
            return await groupPersistance.CreateAsync(item, token);
        }

        /// <inheritdoc />
        public override async Task CreateWithSpecifiedIdAsync(TId masterId, TId slaveId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(slaveId, nameof(slaveId));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(masterId);
            await groupPersistance.CreateWithSpecifiedIdAsync(slaveId, item, token);
        }

        /// <inheritdoc />
        public override async Task<TModel> CreateAndReturnAsync(TId masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(masterId);
            return await groupPersistance.CreateAndReturnAsync(item, token);
        }

        /// <inheritdoc />
        public override async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId masterId, TId slaveId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(slaveId, nameof(slaveId));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(masterId);
            return await groupPersistance.CreateWithSpecifiedIdAndReturnAsync(slaveId, item, token);
        }

        /// <inheritdoc />
        public override async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(TId groupValue, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            if (limit != null)
            {
                InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            }
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.ReadAllWithPagingAsync(offset, limit, token);
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<TModel>> ReadChildrenAsync(TId groupValue, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.ReadAllAsync(limit, token);
        }

        /// <inheritdoc />
        public override async Task<TModel> ReadAsync(TId masterId, TId slaveId, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(slaveId, nameof(slaveId));
            var groupPersistance = GetStorage(masterId);
            return await groupPersistance.ReadAsync(slaveId, token);
        }

        /// <inheritdoc />
        public override async Task DeleteAsync(TId masterId, TId slaveId, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(slaveId, nameof(slaveId));
            var groupPersistance = GetStorage(masterId);
            await groupPersistance.DeleteAsync(slaveId, token);
        }

        /// <inheritdoc />
        public override async Task DeleteChildrenAsync(TId groupValue, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            var groupPersistance = GetStorage(groupValue);
            await groupPersistance.DeleteAllAsync(token);
        }

        /// <summary>
        ///  Get the storage for a specific <paramref name="groupValue"/>.
        /// </summary>
        /// <param name="groupValue"></param>
        /// <returns></returns>
        private CrudMemory<TModelCreate, TModel, TId> GetStorage(TId groupValue)
        {
            if (!Storages.ContainsKey(groupValue)) Storages[groupValue] = new CrudMemory<TModelCreate, TModel, TId>();
            return Storages[groupValue];
        }
    }
}