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
    public abstract class GroupedBase<TItem, TId, TGroup> : IGrouped<TItem, TId, TGroup>
    {
        /// <inheritdoc />
        public async Task<TId> CreateAsync(TGroup groupValue, TItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = StorageHelper.CreateNewId<TId>();
            await CreateWithSpecifiedIdAsync(groupValue, id, item);
            return id;
        }

        /// <inheritdoc />
        public abstract Task CreateWithSpecifiedIdAsync(TGroup groupValue, TId id, TItem item);

        /// <inheritdoc />
        public virtual async Task<TItem> CreateAndReturnAsync(TGroup groupValue, TItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = await CreateAsync(groupValue, item);
            return await ReadAsync(groupValue, id);
        }

        /// <inheritdoc />
        public async Task<TItem> CreateWithSpecifiedIdAndReturnAsync(TGroup groupValue, TId id, TItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            await CreateWithSpecifiedIdAsync(groupValue, id, item);
            return await ReadAsync(groupValue, id);
        }

        /// <inheritdoc />
        public abstract Task<PageEnvelope<TItem>> ReadAllAsync(TGroup groupValue, int offset = 0, int? limit = null);

        /// <inheritdoc />
        public abstract Task<TItem> ReadAsync(TGroup groupValue, TId id);

        /// <inheritdoc />
        public abstract Task DeleteAsync(TGroup groupValue, TId id);

        /// <inheritdoc />
        public virtual async Task DeleteAllAsync(TGroup groupValue)
        {
            var errorMessage = $"The method {nameof(DeleteAllAsync)} of the abstract base class {nameof(GroupedBase<TItem, TId, TGroup>)} must be overridden when it stores items that are not implementing the interface {nameof(IUniquelyIdentifiable<TId>)}";
            FulcrumAssert.IsTrue(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TItem)), null,
                errorMessage);
            var items = new PageEnvelopeEnumerableAsync<TItem>(offset => ReadAllAsync(groupValue, offset));
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
        protected static void MaybeValidate(TItem item)
        {
            StorageHelper.MaybeValidate(item);
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IOptimisticConcurrencyControlByETag"/>
        /// then the Etag of the item is set to a new value.
        /// </summary>
        protected static void MaybeCreateNewEtag(TItem item)
        {
            StorageHelper.MaybeCreateNewEtag(item);
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IUniquelyIdentifiable{TId}"/>
        /// then the Id of the item is set.
        /// </summary>
        protected static void MaybeSetId(TId id, TItem item)
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
        protected static void MaybeUpdateTimeStamps(TItem item, bool updateCreatedToo, DateTimeOffset? timeStamp = null)
        {
            StorageHelper.MaybeUpdateTimeStamps(item, updateCreatedToo, timeStamp);
        }
    }
}
