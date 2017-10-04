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
    public class MemoryGroupStorage<TStorableItem, TId> : IGroupStorage<TStorableItem, TId>
        where TStorableItem : class, IStorableItem<TId>, IOptimisticConcurrencyControlByETag, IDeepCopy<TStorableItem>, IValidatable
    {
        /// <summary>
        /// The storages; One dictionary with a memory storage for each group id.
        /// </summary>
        protected static readonly Dictionary<TId, MemoryStorage<TStorableItem, TId>> Storages = new Dictionary<TId, MemoryStorage<TStorableItem, TId>>();

        /// <inheritdoc />
        public async Task<TStorableItem> CreateAsync(TId groupId, TStorableItem item)
        {
            var groupPersistance = GetStorage(groupId);
            return await groupPersistance.CreateAsync(item);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TStorableItem>> ReadAllAsync(TId groupId)
        {
            var groupPersistance = GetStorage(groupId);
            var list = new List<TStorableItem>();
            var offset = 0;
            var pageEnvelope = await groupPersistance.ReadAllAsync(offset);
            while (pageEnvelope.PageInfo.Returned > 0)
            {
                list.AddRange(pageEnvelope.Data);
                if (pageEnvelope.PageInfo.Returned < pageEnvelope.PageInfo.Limit) break;
                offset += pageEnvelope.PageInfo.Returned;
                pageEnvelope = await groupPersistance.ReadAllAsync(offset);
            }
            return list;
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync(TId groupId)
        {
            var groupPersistance = GetStorage(groupId);
            await groupPersistance.DeleteAllAsync();
        }

        /// <summary>
        ///  Get the storage for a specific <paramref name="groupId"/>.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private MemoryStorage<TStorableItem, TId> GetStorage(TId groupId)
        {
            if (!Storages.ContainsKey(groupId)) Storages[groupId] = new MemoryStorage<TStorableItem, TId>();
            return Storages[groupId];
        }
    }
}