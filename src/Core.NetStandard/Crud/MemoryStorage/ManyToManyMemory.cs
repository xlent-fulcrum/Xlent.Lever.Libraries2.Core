using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using StorageHelper = Xlent.Lever.Libraries2.MoveTo.Core.Crud.Helpers.StorageHelper;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.MemoryStorage
{
    /// <summary>
    /// General class for storing a many to one item in memory.
    /// </summary>
    /// <typeparam name="TManyToManyModel">The model for many-to-many-relation.</typeparam>
    /// <typeparam name="TReferenceModel1">The first model of references.</typeparam>
    /// <typeparam name="TReferenceModel2">The second model of references.</typeparam>
    /// <typeparam name="TId">The type for the id field of the models.</typeparam>
    public class ManyToManyMemory<TManyToManyModel, TReferenceModel1, TReferenceModel2, TId> : CrudMemory<TManyToManyModel, TId>, IManyToManyRelationComplete<TManyToManyModel, TReferenceModel1, TReferenceModel2, TId>
        where TManyToManyModel : class
    {
        private readonly GetForeignKeyDelegate _getForeignKey1Delegate;
        private readonly GetForeignKeyDelegate _getForeignKey2Delegate;
        private readonly ICrd<TReferenceModel1, TId> _foreignHandler1;
        private readonly ICrd<TReferenceModel2, TId> _foreignHandler2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getForeignKey1Delegate">See <see cref="GetForeignKeyDelegate"/>.</param>
        /// <param name="getForeignKey2Delegate">See <see cref="GetForeignKeyDelegate"/>.</param>
        /// <param name="foreignHandler1">Functionality to read a specified parent.</param>
        /// <param name="foreignHandler2">Functionality to read a specified parent.</param>
        public ManyToManyMemory(GetForeignKeyDelegate getForeignKey1Delegate, GetForeignKeyDelegate getForeignKey2Delegate, ICrd<TReferenceModel1, TId> foreignHandler1, ICrd<TReferenceModel2, TId> foreignHandler2)
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
        public async Task<PageEnvelope<TReferenceModel2>> ReadReferencedItemsByReference1WithPagingAsync(TId id, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            return await ReadReferencedItemsByForeignKeyAsync(
                id,
                _getForeignKey1Delegate,
                _getForeignKey2Delegate,
                _foreignHandler2, 
                offset, limit, token);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TReferenceModel2>> ReadReferencedItemsByReference1Async(TId id, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return await StorageHelper.ReadPagesAsync((offset, t) => ReadReferencedItemsByReference1WithPagingAsync(id, offset, null, t), limit, token);
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TReferenceModel1>> ReadReferencedItemsByReference2WithPagingAsync(TId id, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            return await ReadReferencedItemsByForeignKeyAsync(
                id,
                _getForeignKey2Delegate,
                _getForeignKey1Delegate,
                _foreignHandler1,
                offset, limit, token);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TReferenceModel1>> ReadReferencedItemsByReference2Async(TId id, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return await StorageHelper.ReadPagesAsync((offset, t) => ReadReferencedItemsByReference2WithPagingAsync(id, offset, null, t), limit, token);
        }

        /// <inheritdoc />
        public async Task DeleteReferencedItemsByReference1(TId id, CancellationToken token = default(CancellationToken))
        {
            await DeleteReferencedItemsByForeignKey<TReferenceModel2>(id, _getForeignKey1Delegate, _foreignHandler2, token);
        }

        /// <inheritdoc />
        public async Task DeleteReferencedItemsByReference2(TId id, CancellationToken token = default(CancellationToken))
        {
            await DeleteReferencedItemsByForeignKey<TReferenceModel1>(id, _getForeignKey2Delegate, _foreignHandler1, token);
        }
        
        private Task<PageEnvelope<T>> ReadReferencedItemsByForeignKeyAsync<T>(TId id, GetForeignKeyDelegate idDelegate, GetForeignKeyDelegate referenceIdDelegate, IRead<T, TId> referenceHandler, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            throw new FulcrumNotImplementedException("This method needs to be changed and tests");
            //limit = limit ?? PageInfo.DefaultLimit;
            //InternalContract.RequireNotNull(id, nameof(id));
            //InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            //InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            //List<Task<T>> taskList;
            //lock (MemoryItems)
            //{
            //    taskList = MemoryItems.Values
            //        .Where(i => id.Equals(idDelegate(i)))
            //        .Skip(offset)
            //        .Take(limit.Value)
            //        .Select(i => referenceHandler.ReadAsync(referenceIdDelegate(i), token))
            //        .ToList();
            //}

            //await Task.WhenAll(taskList);
            //var list = new List<T>();
            //foreach (var task in taskList)
            //{
            //    list.Add(await task);
            //}
            //var page = new PageEnvelope<T>(offset, limit.Value, null, list);
            //return await Task.FromResult(page);
        }
        
        private Task DeleteReferencedItemsByForeignKey<T>(TId id, GetForeignKeyDelegate idDelegate, IDelete<TId> referenceHandler, CancellationToken token)
        {
            throw new FulcrumNotImplementedException("This method needs to be changed and tests");
            //InternalContract.RequireNotNull(id, nameof(id));
            //var errorMessage = $"{nameof(TManyToManyModel)} must implement the interface {nameof(IUniquelyIdentifiable<TId>)} for this method to work.";
            //InternalContract.Require(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TManyToManyModel)), errorMessage);
            //List<TManyToManyModel> list;
            //lock (MemoryItems)
            //{
            //    list = MemoryItems.Values
            //        .Where(i => id.Equals(idDelegate(i)))
            //        .ToList();
            //}

            //foreach (var item in list)
            //{
            //    if (!(item is IUniquelyIdentifiable<TId> idItem)) continue;
            //    await referenceHandler.DeleteAsync(idItem.Id, token);
            //}
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TManyToManyModel>> ReadByReference1WithPagingAsync(TId reference1Id, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
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
        public async Task<IEnumerable<TManyToManyModel>> ReadByReference1Async(TId reference1Id, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return await StorageHelper.ReadPagesAsync((offset, t) => ReadByReference1WithPagingAsync(reference1Id, offset, null, t), limit, token);
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TManyToManyModel>> ReadByReference2WithPagingAsync(TId reference2Id, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
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
        public async Task<IEnumerable<TManyToManyModel>> ReadByReference2Async(TId reference2Id, int limit = Int32.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return await StorageHelper.ReadPagesAsync((offset, t) => ReadByReference2WithPagingAsync(reference2Id, offset, null, t), limit, token);
        }

        /// <inheritdoc />
        public async Task DeleteByReference1Async(TId reference1Id, CancellationToken token = default(CancellationToken))
        {
            var enumerator = new PageEnvelopeEnumeratorAsync<TManyToManyModel>((offset,t) => ReadByReference1WithPagingAsync(reference1Id, offset, null, t), token);
            await DeleteItemsAsync(enumerator, token);
        }

        /// <inheritdoc />
        public async Task DeleteByReference2Async(TId reference2Id, CancellationToken token = default(CancellationToken))
        {
            var enumerator = new PageEnvelopeEnumeratorAsync<TManyToManyModel>((offset, t) => ReadByReference2WithPagingAsync(reference2Id, offset, null, t), token);
            await DeleteItemsAsync(enumerator, token);
        }

        private async Task DeleteItemsAsync(PageEnvelopeEnumeratorAsync<TManyToManyModel> enumerator, CancellationToken token)
        {
            var tasks = new List<Task>();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                var itemWithId = item as IUniquelyIdentifiable<TId>;
                InternalContract.Require(itemWithId != null,
                    $"The type {typeof(TManyToManyModel).FullName} must implement {typeof(IUniquelyIdentifiable<TId>).Name} for this method to work.");
                if (itemWithId == null) break;
                var task = DeleteAsync(itemWithId.Id, token);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }
    }
}
