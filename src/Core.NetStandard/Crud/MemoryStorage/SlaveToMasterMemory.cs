using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.MoveTo.Core.Crud.Helpers;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.MemoryStorage
{
    /// <summary>
    /// Functionality for persisting objects in groups.
    /// </summary>
    public class SlaveToMasterMemory<TModel, TId, TGroupId> : SlaveToMasterRelationBase<TModel, TId, TGroupId>
    {
        /// <summary>
        /// The storages; One dictionary with a memory storage for each group id.
        /// </summary>
        protected static readonly ConcurrentDictionary<TGroupId, CrudMemory<TModel, TId>> Storages = new ConcurrentDictionary<TGroupId, CrudMemory<TModel, TId>>();

        /// <inheritdoc />
        public override async Task<TId> CreateAsync(TGroupId masterId, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(masterId);
            return await groupPersistance.CreateAsync(item, token);
        }

        /// <inheritdoc />
        public override async Task CreateWithSpecifiedIdAsync(TGroupId masterId, TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(masterId);
            await groupPersistance.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        public override async Task<TModel> CreateAndReturnAsync(TGroupId masterId, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(masterId);
            return await groupPersistance.CreateAndReturnAsync(item, token);
        }

        /// <inheritdoc />
        public override async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TGroupId masterId, TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(masterId);
            return await groupPersistance.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
        }

        /// <inheritdoc />
        public override async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(TGroupId groupValue, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
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
        public override async Task<IEnumerable<TModel>> ReadChildrenAsync(TGroupId groupValue, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.ReadAllAsync(limit, token);
        }

        /// <inheritdoc />
        public override async Task<TModel> ReadAsync(TGroupId masterId, TId id, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var groupPersistance = GetStorage(masterId);
            return await groupPersistance.ReadAsync(id);
        }

        /// <inheritdoc />
        public override async Task DeleteAsync(TGroupId masterId, TId id, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var groupPersistance = GetStorage(masterId);
            await groupPersistance.DeleteAsync(id);
        }

        /// <inheritdoc />
        public override async Task DeleteChildrenAsync(TGroupId groupValue, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            var groupPersistance = GetStorage(groupValue);
            await groupPersistance.DeleteAllAsync();
        }

        /// <summary>
        ///  Get the storage for a specific <paramref name="groupValue"/>.
        /// </summary>
        /// <param name="groupValue"></param>
        /// <returns></returns>
        private CrudMemory<TModel, TId> GetStorage(TGroupId groupValue)
        {
            if (!Storages.ContainsKey(groupValue)) Storages[groupValue] = new CrudMemory<TModel, TId>();
            return Storages[groupValue];
        }
    }
}