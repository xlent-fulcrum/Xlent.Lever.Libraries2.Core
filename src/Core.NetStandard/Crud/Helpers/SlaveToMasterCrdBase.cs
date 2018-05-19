using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Helpers
{
    /// <inheritdoc cref="SlaveToMasterCrdBase{TModelCreate,TModel,TId}" />
    public abstract class SlaveToMasterCrdBase<TModel, TId> :
        SlaveToMasterCrdBase<TModel, TModel, TId>,
        ISlaveToMasterCrd<TModel, TId>
    {
    }

    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="CreateAndReturnAsync"/>,
    /// <see cref="CreateWithSpecifiedIdAndReturnAsync"/>, <see cref="DeleteChildrenAsync"/>
    /// and <see cref="DeleteAllAsync"/>.
    /// </summary>
    public abstract class SlaveToMasterCrdBase<TModelCreate, TModel, TId> :
        SlaveToMasterReadBase<TModel, TId>,
        ISlaveToMasterCrd<TModelCreate, TModel, TId>
        where TModel : TModelCreate
    {
        /// <inheritdoc />
        public abstract Task<SlaveToMasterId<TId>> CreateAsync(TId masterId, TModelCreate item,
        CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TId masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(masterId, nameof(masterId));
            InternalContract.RequireNotNull(item, nameof(item));
            InternalContract.RequireValidated(item, nameof(item));
            var id = await CreateAsync(masterId, item, token);
            return await ReadAsync(id, token);
        }

        /// <inheritdoc />
        public abstract Task CreateWithSpecifiedIdAsync(SlaveToMasterId<TId> id, TModelCreate item,
            CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(SlaveToMasterId<TId> id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(id, nameof(id));
            InternalContract.RequireValidated(id, nameof(id));
            InternalContract.RequireNotNull(item, nameof(item));
            InternalContract.RequireValidated(item, nameof(item));
            await CreateWithSpecifiedIdAsync(id, item, token);
            return await ReadAsync(id, token);
        }

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
