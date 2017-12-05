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
        public abstract Task<TId> CreateAsync(TItem item);

        /// <inheritdoc />
        public virtual async Task<TItem> CreateAndReturnAsync(TItem item)
        {
            InternalContract.RequireNotNull(item, nameof(item));
            if (item is IValidatable validatable) InternalContract.RequireValidated(validatable, nameof(item));
            var id = await CreateAsync(item);
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
            var errorMessage = $"The method {nameof(DeleteAllAsync)} of the abstract base class {nameof(CrdBase<TItem, TId>)} must be overridden when it stores items that are not implementing the interface {nameof(IIdentifiable<TId>)}";
            FulcrumAssert.IsTrue(typeof(IIdentifiable<TId>).IsAssignableFrom(typeof(TItem)), null, 
                errorMessage);
            var items = new PageEnvelopeEnumerableAsync<TItem>(offset => ReadAllAsync(offset));
            var enumerator = items.GetEnumerator();
            var taskList = new List<Task>();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                var identifiable = item as IIdentifiable<TId>;
                FulcrumAssert.IsNotNull(identifiable, null, errorMessage);
                if (identifiable == null) continue;
                taskList.Add(DeleteAsync(identifiable.Id));
            }
            await Task.WhenAll(taskList);
        }
    }
}
