using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// Functionality for persisting objects in groups.
    /// </summary>
    public class MemoryGroupPersistance<TStorableItem, TId, TGroup> : IGrouped<TStorableItem, TId, TGroup>
        where TStorableItem : class, IUniquelyIdentifiable<TId>, IOptimisticConcurrencyControlByETag, IValidatable
    {
        /// <summary>
        /// The storages; One dictionary with a memory storage for each group id.
        /// </summary>
        protected static readonly ConcurrentDictionary<TGroup, MemoryPersistance<TStorableItem, TId>> Storages = new ConcurrentDictionary<TGroup, MemoryPersistance<TStorableItem, TId>>();

        /// <inheritdoc />
        public async Task<TId> CreateAsync(TGroup groupValue, TStorableItem item)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.CreateAsync(item);
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(TGroup groupValue, TId id, TStorableItem item)
        {
            var groupPersistance = GetStorage(groupValue);
            await groupPersistance.CreateWithSpecifiedIdAsync(id, item);
        }

        /// <inheritdoc />
        public async Task<TStorableItem> CreateAndReturnAsync(TGroup groupValue, TStorableItem item)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.CreateAndReturnAsync(item);
        }

        /// <inheritdoc />
        public async Task<TStorableItem> CreateWithSpecifiedIdAndReturnAsync(TGroup groupValue, TId id, TStorableItem item)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.CreateWithSpecifiedIdAndReturnAsync(id, item);
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TStorableItem>> ReadAllAsync(TGroup groupValue, int offset = 0, int? limit = null)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.ReadAllWithPagingAsync(offset);
        }

        /// <inheritdoc />
        public async Task<TStorableItem> ReadAsync(TGroup groupValue, TId id)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.ReadAsync(id);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TGroup groupValue, TId id)
        {
            var groupPersistance = GetStorage(groupValue);
            await groupPersistance.DeleteAsync(id);
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync(TGroup groupValue)
        {
            var groupPersistance = GetStorage(groupValue);
            await groupPersistance.DeleteAllAsync();
        }

        /// <summary>
        ///  Get the storage for a specific <paramref name="groupValue"/>.
        /// </summary>
        /// <param name="groupValue"></param>
        /// <returns></returns>
        private MemoryPersistance<TStorableItem, TId> GetStorage(TGroup groupValue)
        {
            if (!Storages.ContainsKey(groupValue)) Storages[groupValue] = new MemoryPersistance<TStorableItem, TId>();
            return Storages[groupValue];
        }
    }
}