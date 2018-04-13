using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Helpers
{
    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="CreateAndReturnAsync"/>,
    /// and <see cref="DeleteChildrenAsync"/>.
    /// </summary>
    public abstract class SlaveToMasterRelationBase<TModel, TId, TGroupId> : ISlaveToMasterRelation<TModel, TId, TGroupId>
    {
        /// <inheritdoc />
        public virtual async Task<TId> CreateAsync(TGroupId masterId, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = StorageHelper.CreateNewId<TId>();
            await CreateWithSpecifiedIdAsync(masterId, id, item, token);
            return id;
        }

        /// <inheritdoc />
        public abstract Task CreateWithSpecifiedIdAsync(TGroupId masterId, TId id, TModel item, CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task<TModel> CreateAndReturnAsync(TGroupId masterId, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = await CreateAsync(masterId, item, token);
            return await ReadAsync(masterId, id, token);
        }

        /// <inheritdoc />
        public virtual async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TGroupId masterId, TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            await CreateWithSpecifiedIdAsync(masterId, id, item, token);
            return await ReadAsync(masterId, id, token);
        }

        /// <inheritdoc />
        public abstract Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(TGroupId groupValue, int offset, int? limit = null, CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TModel>> ReadChildrenAsync(TGroupId groupValue, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            return await StorageHelper.ReadPagesAsync((offset,ct) => ReadChildrenWithPagingAsync(groupValue, offset, limit, ct), limit, token);
        }

        /// <inheritdoc />
        public abstract Task<TModel> ReadAsync(TGroupId masterId, TId id, CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public abstract Task DeleteAsync(TGroupId masterId, TId id, CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task DeleteChildrenAsync(TGroupId groupValue, CancellationToken token = default(CancellationToken))
        {
            var errorMessage = $"The method {nameof(DeleteChildrenAsync)} of the abstract base class {nameof(SlaveToMasterRelationBase<TModel, TId, TGroupId>)} must be overridden when it stores items that are not implementing the interface {nameof(IUniquelyIdentifiable<TId>)}";
            FulcrumAssert.IsTrue(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TModel)), null,
                errorMessage);
            var items = new PageEnvelopeEnumerableAsync<TModel>((offset,ct) => ReadChildrenWithPagingAsync(groupValue, offset, null, ct));
            var enumerator = items.GetEnumerator();
            var taskList = new List<Task>();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                var identifiable = item as IUniquelyIdentifiable<TId>;
                FulcrumAssert.IsNotNull(identifiable, null, errorMessage);
                if (identifiable == null) continue;
                taskList.Add(DeleteAsync(groupValue, identifiable.Id, token));
            }
            await Task.WhenAll(taskList);
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IValidatable"/>, then it is validated.
        /// </summary>
        protected static void MaybeValidate(TModel item)
        {
            StorageHelper.MaybeValidate(item);
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IOptimisticConcurrencyControlByETag"/>
        /// then the Etag of the item is set to a new value.
        /// </summary>
        protected static void MaybeCreateNewEtag(TModel item)
        {
            StorageHelper.MaybeCreateNewEtag(item);
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IUniquelyIdentifiable{TId}"/>
        /// then the Id of the item is set.
        /// </summary>
        protected static void MaybeSetId(TId id, TModel item)
        {
            StorageHelper.MaybeSetId(id, item);
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="ITimeStamped"/>
        /// then the <see cref="ITimeStamped.RecordUpdatedAt"/> is set. If <paramref name="updateCreatedToo"/> is true, 
        /// then the <see cref="ITimeStamped.RecordCreatedAt"/> is also set.
        /// </summary>
        /// <param name="item">The item that will be affected.</param>
        /// <param name="updateCreatedToo">True means that we should update the create property too.</param>
        /// <param name="timeStamp">Optional time stamp to use when setting the time properties. If null, then 
        /// <see cref="DateTimeOffset.Now"/> will be used.</param>
        protected static void MaybeUpdateTimeStamps(TModel item, bool updateCreatedToo, DateTimeOffset? timeStamp = null)
        {
            StorageHelper.MaybeUpdateTimeStamps(item, updateCreatedToo, timeStamp);
        }
    }
}
