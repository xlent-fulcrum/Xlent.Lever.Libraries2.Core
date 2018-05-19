using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Helpers
{
    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="ReadChildrenAsync"/>
    /// and <see cref="DeleteChildrenAsync"/>.
    /// </summary>
    public abstract class SlaveToMasterRudBase<TModel, TId> :
        SlaveToMasterReadBase<TModel, TId>,
        ISlaveToMasterRud<TModel, TId>
    {
        /// <inheritdoc />
        public abstract Task UpdateAsync(SlaveToMasterId<TId> id, TModel item,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task<TModel> UpdateAndReturnAsync(SlaveToMasterId<TId> id, TModel item, CancellationToken token = default(CancellationToken))
        {
            await UpdateAsync(id, item, token);
            return await ReadAsync(id, token);
        }

        /// <inheritdoc />
        public abstract Task<Lock> ClaimLockAsync(SlaveToMasterId<TId> id,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public abstract Task ReleaseLockAsync(Lock @lock, CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public abstract Task DeleteAsync(SlaveToMasterId<TId> id, CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task DeleteChildrenAsync(TId masterId, CancellationToken token = default(CancellationToken))
        {
            var errorMessage = $"The method {nameof(DeleteChildrenAsync)} of the abstract base class {nameof(SlaveToMasterRudBase<TModel, TId>)} must be overridden when it stores items that are not implementing the interface {nameof(IUniquelyIdentifiable<SlaveToMasterId<TId>>)}";
            FulcrumAssert.IsTrue(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TModel)), null,
                errorMessage);
            var items = new PageEnvelopeEnumerableAsync<TModel>((offset, ct) => ReadChildrenWithPagingAsync(masterId, offset, null, ct), token);
            var enumerator = items.GetEnumerator();
            await DeleteItems(token, enumerator, errorMessage);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            var errorMessage = $"The method {nameof(DeleteAllAsync)} of the abstract base class {nameof(SlaveToMasterRudBase<TModel, TId>)} must be overridden when it stores items that are not implementing the interface {nameof(IUniquelyIdentifiable<SlaveToMasterId<TId>>)}";
            FulcrumAssert.IsTrue(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TModel)), null,
                errorMessage);
            var items = new PageEnvelopeEnumerableAsync<TModel>((offset, ct) => ReadAllWithPagingAsync(offset, null, ct), token);
            var enumerator = items.GetEnumerator();
            await DeleteItems(token, enumerator, errorMessage);
        }

        private async Task DeleteItems(CancellationToken token, PageEnvelopeEnumeratorAsync<TModel> enumerator, string errorMessage)
        {
            var taskList = new List<Task>();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                var identifiable = item as IUniquelyIdentifiable<SlaveToMasterId<TId>>;
                FulcrumAssert.IsNotNull(identifiable, null, errorMessage);
                if (identifiable == null) continue;
                taskList.Add(DeleteAsync(identifiable.Id, token));
            }

            await Task.WhenAll(taskList);
        }
    }
}
