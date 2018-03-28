using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="CreateAndReturnAsync"/>,
    /// and <see cref="DeleteAllAsync"/>.
    /// </summary>
    public abstract class GroupedBase<TModel, TId, TGroupId> : IGrouped<TModel, TId, TGroupId>
    {
        /// <inheritdoc />
        public async Task<TId> CreateAsync(TGroupId groupValue, TModel item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = StorageHelper.CreateNewId<TId>();
            await CreateWithSpecifiedIdAsync(groupValue, id, item);
            return id;
        }

        /// <inheritdoc />
        public abstract Task CreateWithSpecifiedIdAsync(TGroupId groupValue, TId id, TModel item);

        /// <inheritdoc />
        public virtual async Task<TModel> CreateAndReturnAsync(TGroupId groupValue, TModel item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = await CreateAsync(groupValue, item);
            return await ReadAsync(groupValue, id);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TGroupId groupValue, TId id, TModel item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            await CreateWithSpecifiedIdAsync(groupValue, id, item);
            return await ReadAsync(groupValue, id);
        }

        /// <inheritdoc />
        public abstract Task<PageEnvelope<TModel>> ReadAllAsync(TGroupId groupValue, int offset = 0, int? limit = null);

        /// <inheritdoc />
        public abstract Task<TModel> ReadAsync(TGroupId groupValue, TId id);

        /// <inheritdoc />
        public abstract Task DeleteAsync(TGroupId groupValue, TId id);

        /// <inheritdoc />
        public virtual async Task DeleteAllAsync(TGroupId groupValue)
        {
            var errorMessage = $"The method {nameof(DeleteAllAsync)} of the abstract base class {nameof(GroupedBase<TModel, TId, TGroupId>)} must be overridden when it stores items that are not implementing the interface {nameof(IUniquelyIdentifiable<TId>)}";
            FulcrumAssert.IsTrue(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TModel)), null,
                errorMessage);
            var items = new PageEnvelopeEnumerableAsync<TModel>(offset => ReadAllAsync(groupValue, offset));
            var enumerator = items.GetEnumerator();
            var taskList = new List<Task>();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                var identifiable = item as IUniquelyIdentifiable<TId>;
                FulcrumAssert.IsNotNull(identifiable, null, errorMessage);
                if (identifiable == null) continue;
                taskList.Add(DeleteAsync(groupValue, identifiable.Id));
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
