using System.Collections.Generic;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Misc.Models;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// Functionality for persisting objects in groups.
    /// </summary>
    public class MemoryGroupPersistance<TStorableItem, TId, TGroup> : IGrouped<TStorableItem, TId, TGroup>
        where TStorableItem : class, IStorableItem<TId>, IOptimisticConcurrencyControlByETag, IDeepCopy<TStorableItem>, IValidatable
    {
        /// <summary>
        /// The storages; One dictionary with a memory storage for each group id.
        /// </summary>
        protected static readonly Dictionary<TGroup, MemoryPersistance<TStorableItem, TId>> Storages = new Dictionary<TGroup, MemoryPersistance<TStorableItem, TId>>();

        /// <inheritdoc />
        public async Task<TStorableItem> CreateAsync(TGroup groupValue, TStorableItem item)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.CreateAsync(item);
        }

        /// <inheritdoc />
        public async Task<IPageEnvelope<TStorableItem, TId>> ReadAllAsync(TGroup groupValue, int offset = 0, int? limit = null)
        {
            var groupPersistance = GetStorage(groupValue);
            return await groupPersistance.ReadAllAsync(offset);
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