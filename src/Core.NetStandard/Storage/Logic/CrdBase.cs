using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="CreateAndReturnAsync"/>,
    /// and <see cref="DeleteAllAsync"/>.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class CrdBase<TItem, TId> : ICrd<TItem, TId>
    {
        /// <inheritdoc />
        public virtual async Task<TId> CreateAsync(TItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = StorageHelper.CreateNewId<TId>();
            await CreateWithSpecifiedIdAsync(id, item);
            return id;
        }

        /// <inheritdoc />
        public virtual async Task<TItem> CreateAndReturnAsync(TItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            var id = await CreateAsync(item);
            return await ReadAsync(id);
        }

        /// <inheritdoc />
        public virtual Task CreateWithSpecifiedIdAsync(TId id, TItem item)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<TItem> CreateWithSpecifiedIdAndReturnAsync(TId id, TItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            await CreateWithSpecifiedIdAsync(id, item);
            return await ReadAsync(id);
        }

        /// <inheritdoc />
        public abstract Task<TItem> ReadAsync(TId id);

        /// <inheritdoc />
        public abstract Task DeleteAsync(TId id);

        /// <inheritdoc />
        public abstract Task<PageEnvelope<TItem>> ReadAllAsync(int offset = 0, int? limit = null);

        /// <inheritdoc />
        public virtual async Task DeleteAllAsync()
        {
            var errorMessage = $"The method {nameof(DeleteAllAsync)} of the abstract base class {nameof(CrdBase<TItem, TId>)} must be overridden when it stores items that are not implementing the interface {nameof(IUniquelyIdentifiable<TId>)}";
            FulcrumAssert.IsTrue(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TItem)), null, 
                errorMessage);
            var items = new PageEnvelopeEnumerableAsync<TItem>(offset => ReadAllAsync(offset));
            var enumerator = items.GetEnumerator();
            var taskList = new List<Task>();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                var identifiable = item as IUniquelyIdentifiable<TId>;
                FulcrumAssert.IsNotNull(identifiable, null, errorMessage);
                if (identifiable == null) continue;
                taskList.Add(DeleteAsync(identifiable.Id));
            }
            await Task.WhenAll(taskList);
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IValidatable"/>, then it is validated.
        /// </summary>
        protected static void MaybeValidate(TItem item)
        {
            if (item is IValidatable validatable) InternalContract.RequireValidated(validatable, nameof(item));
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IOptimisticConcurrencyControlByETag"/>
        /// then the old value is read using <see cref="ReadAsync"/> and the values are verified to be equal.
        /// The Etag of the item is then set to a new value.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        protected async Task MaybeVerifyEtagForUpdateAsync(TId id, TItem item)
        {
            if (item is IOptimisticConcurrencyControlByETag etaggable)
            {
                var oldItem = await ReadAsync(id);
                if (oldItem != null)
                {
                    var oldEtag = (oldItem as IOptimisticConcurrencyControlByETag)?.Etag;
                    if (oldEtag?.ToLowerInvariant() != etaggable.Etag?.ToLowerInvariant())
                        throw new FulcrumConflictException($"The updated item ({item}) had an old ETag value.");
                }
            }
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IOptimisticConcurrencyControlByETag"/>
        /// then the Etag of the item is set to a new value.
        /// </summary>
        protected static void MaybeCreateNewEtag(TItem item)
        {
            if (item is IOptimisticConcurrencyControlByETag eTaggable) eTaggable.Etag = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// If <paramref name="item"/> implements <see cref="IUniquelyIdentifiable{TId}"/>
        /// then the Id of the item is set.
        /// </summary>
        protected static void MaybeSetId(TId id, TItem item)
        {
            if (item is IUniquelyIdentifiable<TId> identifiable) identifiable.Id = id;
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
            if (!(item is ITimeStamped timeStamped)) return;
            timeStamp = timeStamp ?? DateTimeOffset.Now;
            timeStamped.RecordUpdatedAt = timeStamp.Value;
            if (updateCreatedToo) timeStamped.RecordCreatedAt = timeStamp.Value;
        }
    }
}
