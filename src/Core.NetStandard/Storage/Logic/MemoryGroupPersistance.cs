using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// Functionality for persisting objects in groups.
    /// </summary>
    public class MemoryGroupPersistance<TModel, TId, TGroupId> : GroupedBase<TModel, TId, TGroupId>
    {
        /// <summary>
        /// The storages; One dictionary with a memory storage for each group id.
        /// </summary>
        protected static readonly ConcurrentDictionary<TGroupId, MemoryPersistance<TModel, TId>> Storages = new ConcurrentDictionary<TGroupId, MemoryPersistance<TModel, TId>>();

        /// <inheritdoc />
        public override async Task<TId> CreateAsync(TGroupId groupValue, TModel item)
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.CreateAsync(item);
        }

        /// <inheritdoc />
        public override async Task CreateWithSpecifiedIdAsync(TGroupId groupValue, TId id, TModel item)
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(groupValue);
            await groupPersistance.CreateWithSpecifiedIdAsync(id, item);
        }

        /// <inheritdoc />
        public override async Task<TModel> CreateAndReturnAsync(TGroupId groupValue, TModel item)
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.CreateAndReturnAsync(item);
        }

        /// <inheritdoc />
        public override async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TGroupId groupValue, TId id, TModel item)
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotDefaultValue(item, nameof(item));
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.CreateWithSpecifiedIdAndReturnAsync(id, item);
        }

        /// <inheritdoc />
        public override async Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(TGroupId groupValue, int offset = 0, int? limit = null)
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            if (limit != null)
            {
                InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            }
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.ReadAllWithPagingAsync(offset);
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<TModel>> ReadAllAsync(TGroupId groupValue, int limit = int.MaxValue)
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.ReadAllAsync(limit);
        }

        /// <inheritdoc />
        public override async Task<TModel> ReadAsync(TGroupId groupValue, TId id)
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.ReadAsync(id);
        }

        /// <inheritdoc />
        public override async Task DeleteAsync(TGroupId groupValue, TId id)
        {
            InternalContract.RequireNotDefaultValue(groupValue, nameof(groupValue));
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var groupPersistance = GetStorage(groupValue);
            await groupPersistance.DeleteAsync(id);
        }

        /// <inheritdoc />
        public override async Task DeleteAllAsync(TGroupId groupValue)
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
        private MemoryPersistance<TModel, TId> GetStorage(TGroupId groupValue)
        {
            if (!Storages.ContainsKey(groupValue)) Storages[groupValue] = new MemoryPersistance<TModel, TId>();
            return Storages[groupValue];
        }
    }
}