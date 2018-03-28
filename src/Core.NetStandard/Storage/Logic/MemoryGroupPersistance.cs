using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// Functionality for persisting objects in groups.
    /// </summary>
    public class MemoryGroupPersistance<TModel, TId, TGroupId> : IGrouped<TModel, TId, TGroupId>
        where TModel : class, IUniquelyIdentifiable<TId>, IOptimisticConcurrencyControlByETag, IValidatable
    {
        /// <summary>
        /// The storages; One dictionary with a memory storage for each group id.
        /// </summary>
        protected static readonly ConcurrentDictionary<TGroupId, MemoryPersistance<TModel, TId>> Storages = new ConcurrentDictionary<TGroupId, MemoryPersistance<TModel, TId>>();

        /// <inheritdoc />
        public async Task<TId> CreateAsync(TGroupId groupValue, TModel item)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.CreateAsync(item);
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(TGroupId groupValue, TId id, TModel item)
        {
            var groupPersistance = GetStorage(groupValue);
            await groupPersistance.CreateWithSpecifiedIdAsync(id, item);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TGroupId groupValue, TModel item)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.CreateAndReturnAsync(item);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TGroupId groupValue, TId id, TModel item)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.CreateWithSpecifiedIdAndReturnAsync(id, item);
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TModel>> ReadAllAsync(TGroupId groupValue, int offset = 0, int? limit = null)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.ReadAllWithPagingAsync(offset);
        }

        /// <inheritdoc />
        public async Task<TModel> ReadAsync(TGroupId groupValue, TId id)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.ReadAsync(id);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TGroupId groupValue, TId id)
        {
            var groupPersistance = GetStorage(groupValue);
            await groupPersistance.DeleteAsync(id);
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync(TGroupId groupValue)
        {
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