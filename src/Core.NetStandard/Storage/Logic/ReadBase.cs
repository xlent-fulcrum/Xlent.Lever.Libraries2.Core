using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// Abstract base class that has a default implementation for <see cref="ReadAllAsync"/>.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class ReadBase<TModel, TId> : IReadAll<TModel, TId>
    {

        /// <inheritdoc />
        public abstract Task<TModel> ReadAsync(TId id);

        /// <inheritdoc />
        public abstract Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset = 0, int? limit = null);

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int limit = int.MaxValue)
        {
            var result = new List<TModel>();
            var offset = 0;
            while (true)
            {
                var page = await ReadAllWithPagingAsync(offset);
                if (page.PageInfo.Returned == 0) break;
                var returned = page.PageInfo.Returned;
                if (offset + returned > limit) returned = limit - offset;
                result.AddRange(page.Data.Take(returned));
                offset += page.PageInfo.Returned;
            }

            return result;
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

        /// <summary>
        /// Helper method to convert from one parameter type to another.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected static T ConvertToParameterType<T>(Object source)
        {
            return StorageHelper.ConvertToParameterType<T>(source);
        }
    }
}
