using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// General class for storing a many to one item in memory.
    /// </summary>
    /// <typeparam name="TManyToManyModel">The model for many-to-many-relation.</typeparam>
    /// <typeparam name="TReferenceModel1">The first model of references.</typeparam>
    /// <typeparam name="TReferenceModel2">The second model of references.</typeparam>
    /// <typeparam name="TId">The type for the id field of the models.</typeparam>
    public class MemoryManyToManyPersistancer<TManyToManyModel, TReferenceModel1, TReferenceModel2, TId> : MemoryPersistance<TManyToManyModel, TId>, IManyToManyRelation<TReferenceModel1, TReferenceModel2, TId> where TManyToManyModel : class
    {
        private readonly GetForeignKeyDelegate _getForeignKey1Delegate;
        private readonly GetForeignKeyDelegate _getForeignKey2Delegate;
        private readonly IRead<TReferenceModel1, TId> _foreignHandler1;
        private readonly IRead<TReferenceModel2, TId> _foreignHandler2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getForeignKey1Delegate">See <see cref="GetForeignKeyDelegate"/>.</param>
        /// <param name="getForeignKey2Delegate">See <see cref="GetForeignKeyDelegate"/>.</param>
        /// <param name="foreignHandler1">Functionality to read a specified parent.</param>
        /// <param name="foreignHandler2">Functionality to read a specified parent.</param>
        public MemoryManyToManyPersistancer(GetForeignKeyDelegate getForeignKey1Delegate, GetForeignKeyDelegate getForeignKey2Delegate, IRead<TReferenceModel1, TId> foreignHandler1, IRead<TReferenceModel2, TId> foreignHandler2)
        {
            _getForeignKey1Delegate = getForeignKey1Delegate;
            _getForeignKey2Delegate = getForeignKey2Delegate;
            _foreignHandler1 = foreignHandler1;
            _foreignHandler2 = foreignHandler2;
        }

        /// <summary>
        /// A delegate method for getting a foreign key id from a stored item.
        /// </summary>
        /// <param name="item">The item to get the parent for.</param>
        public delegate TId GetForeignKeyDelegate(TManyToManyModel item);

        /// <inheritdoc />
        public async Task<PageEnvelope<TReferenceModel2>> ReadReferencedItemsByForeignKey1(TId id, int offset = 0, int? limit = null)
        {
            return await ReadReferencedItemsByForeignKey(
                id,
                _getForeignKey1Delegate,
                _getForeignKey2Delegate,
                _foreignHandler2, 
                offset, limit);
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TReferenceModel1>> ReadReferencedItemsByForeignKey2(TId id, int offset = 0, int? limit = null)
        {
            return await ReadReferencedItemsByForeignKey(
                id,
                _getForeignKey2Delegate,
                _getForeignKey1Delegate,
                _foreignHandler1,
                offset, limit);
        }

        /// <inheritdoc />
        public async Task DeleteReferencesByForeignKey1(TId id)
        {
            await DeleteReferencesByForeignKey(id, _getForeignKey1Delegate);
        }

        /// <inheritdoc />
        public async Task DeleteReferencesByForeignKey2(TId id)
        {
            await DeleteReferencesByForeignKey(id, _getForeignKey2Delegate);
        }
        
        private async Task<PageEnvelope<T>> ReadReferencedItemsByForeignKey<T>(TId id, GetForeignKeyDelegate idDelegate, GetForeignKeyDelegate referenceIdDelegate, IRead<T, TId> referenceHandler, int offset = 0, int? limit = null)
        {
            limit = limit ?? PageInfo.DefaultLimit;
            InternalContract.RequireNotNull(id, nameof(id));
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            List<Task<T>> taskList;
            lock (MemoryItems)
            {
                taskList = MemoryItems.Values
                    .Where(i => id.Equals(idDelegate(i)))
                    .Skip(offset)
                    .Take(limit.Value)
                    .Select(i => referenceHandler.ReadAsync(referenceIdDelegate(i)))
                    .ToList();
            }

            await Task.WhenAll(taskList);
            var list = new List<T>();
            foreach (var task in taskList)
            {
                list.Add(await task);
            }
            var page = new PageEnvelope<T>(offset, limit.Value, null, list);
            return await Task.FromResult(page);
        }
        
        private async Task DeleteReferencesByForeignKey(TId id, GetForeignKeyDelegate idDelegate)
        {
            InternalContract.RequireNotNull(id, nameof(id));
            var errorMessage = $"{nameof(TManyToManyModel)} must implement the interface {nameof(IUniquelyIdentifiable<TId>)} for this method to work.";
            InternalContract.Require(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TManyToManyModel)), errorMessage);
            List<TManyToManyModel> list;
            lock (MemoryItems)
            {
                list = MemoryItems.Values
                    .Where(i => id.Equals(idDelegate(i)))
                    .ToList();
            }

            foreach (var item in list)
            {
                if (!(item is IUniquelyIdentifiable<TId> idItem)) continue;
                await DeleteAsync(idItem.Id);
            }
        }
    }
}
