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
        /// If <paramref name="item"/> implmenents <see cref="IValidatable"/>, then it is validated.
        /// </summary>
        protected static void MaybeValidate(TItem item)
        {
            if (item is IValidatable validatable) InternalContract.RequireValidated(validatable, nameof(item));
        }
    }
}
