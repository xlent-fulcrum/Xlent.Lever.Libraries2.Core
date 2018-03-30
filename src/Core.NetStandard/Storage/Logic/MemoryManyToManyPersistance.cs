using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MemoryManyToManyPersistance<TManyToManyModel, TReferenceModel1, TReferenceModel2, TId> : MemoryPersistance<TManyToManyModel, TId>, IManyToManyRelationComplete<TManyToManyModel, TReferenceModel1, TReferenceModel2, TId>
        where TManyToManyModel : class
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
        public MemoryManyToManyPersistance(GetForeignKeyDelegate getForeignKey1Delegate, GetForeignKeyDelegate getForeignKey2Delegate, IRead<TReferenceModel1, TId> foreignHandler1, IRead<TReferenceModel2, TId> foreignHandler2)
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
        public async Task<PageEnvelope<TReferenceModel2>> ReadReferencedItemsByReference1WithPagingAsync(TId id, int offset, int? limit = null)
        {
            return await ReadReferencedItemsByForeignKeyAsync(
                id,
                _getForeignKey1Delegate,
                _getForeignKey2Delegate,
                _foreignHandler2, 
                offset, limit);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TReferenceModel2>> ReadReferencedItemsByReference1Async(TId id, int limit = Int32.MaxValue)
        {
            return await StorageHelper.ReadPages(offset => ReadReferencedItemsByReference1WithPagingAsync(id, offset));
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TReferenceModel1>> ReadReferencedItemsByReference2WithPagingAsync(TId id, int offset, int? limit = null)
        {
            return await ReadReferencedItemsByForeignKeyAsync(
                id,
                _getForeignKey2Delegate,
                _getForeignKey1Delegate,
                _foreignHandler1,
                offset, limit);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TReferenceModel1>> ReadReferencedItemsByReference2Async(TId id, int limit = Int32.MaxValue)
        {
            return await StorageHelper.ReadPages(offset => ReadReferencedItemsByReference2WithPagingAsync(id, offset));
        }

        /// <inheritdoc />
        public async Task DeleteReferencesByReference1(TId id)
        {
            await DeleteReferencesByForeignKey(id, _getForeignKey1Delegate);
        }

        /// <inheritdoc />
        public async Task DeleteReferencesByReference2(TId id)
        {
            await DeleteReferencesByForeignKey(id, _getForeignKey2Delegate);
        }
        
        private async Task<PageEnvelope<T>> ReadReferencedItemsByForeignKeyAsync<T>(TId id, GetForeignKeyDelegate idDelegate, GetForeignKeyDelegate referenceIdDelegate, IRead<T, TId> referenceHandler, int offset, int? limit = null)
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

        /// <inheritdoc />
        public async Task<PageEnvelope<TManyToManyModel>> ReadByReference1WithPagingAsync(TId reference1Id, int offset, int? limit = null)
        {
            limit = limit ?? PageInfo.DefaultLimit;
            InternalContract.RequireNotNull(reference1Id, nameof(reference1Id));
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            List<TManyToManyModel> list;
            lock (MemoryItems)
            {
                list = MemoryItems.Values
                    .Where(i => reference1Id.Equals(_getForeignKey1Delegate(i)))
                    .Skip(offset)
                    .Take(limit.Value)
                    .ToList();
            }
            var page = new PageEnvelope<TManyToManyModel>(offset, limit.Value, null, list);
            return await Task.FromResult(page);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TManyToManyModel>> ReadByReference1Async(TId reference1Id, int limit = Int32.MaxValue)
        {
            return await StorageHelper.ReadPages(offset => ReadByReference1WithPagingAsync(reference1Id, offset));
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TManyToManyModel>> ReadByReference2WithPagingAsync(TId reference2Id, int offset, int? limit = null)
        {
            limit = limit ?? PageInfo.DefaultLimit;
            InternalContract.RequireNotNull(reference2Id, nameof(reference2Id));
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            List<TManyToManyModel> list;
            lock (MemoryItems)
            {
                list = MemoryItems.Values
                    .Where(i => reference2Id.Equals(_getForeignKey2Delegate(i)))
                    .Skip(offset)
                    .Take(limit.Value)
                    .ToList();
            }
            var page = new PageEnvelope<TManyToManyModel>(offset, limit.Value, null, list);
            return await Task.FromResult(page);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TManyToManyModel>> ReadByReference2Async(TId reference2Id, int limit = Int32.MaxValue)
        {
            return await StorageHelper.ReadPages(offset => ReadByReference2WithPagingAsync(reference2Id, offset));
        }

        /// <inheritdoc />
        public async Task DeleteByReference1Async(TId reference1Id)
        {
            var enumerator = new PageEnvelopeEnumeratorAsync<TManyToManyModel>(offset => ReadByReference1WithPagingAsync(reference1Id, offset));
            var tasks = new List<Task>();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                var itemWithId = item as IUniquelyIdentifiable<TId>;
                InternalContract.Require(itemWithId != null, $"The type {typeof(TManyToManyModel).FullName} must implement {typeof(IUniquelyIdentifiable<TId>).Name} for this method to work.");
                if (itemWithId == null) break;
                var task = DeleteAsync(itemWithId.Id);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        /// <inheritdoc />
        public async Task DeleteByReference2Async(TId reference2Id)
        {
            var enumerator = new PageEnvelopeEnumeratorAsync<TManyToManyModel>(offset => ReadByReference2WithPagingAsync(reference2Id, offset));
            var tasks = new List<Task>();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                var itemWithId = item as IUniquelyIdentifiable<TId>;
                InternalContract.Require(itemWithId != null, $"The type {typeof(TManyToManyModel).FullName} must implement {typeof(IUniquelyIdentifiable<TId>).Name} for this method to work.");
                if (itemWithId == null) break;
                var task = DeleteAsync(itemWithId.Id);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }
    }
}
