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
        public abstract Task<TId> CreateAsync(TGroup groupValue, TItem item);

        /// <inheritdoc />
        public virtual async Task<TItem> CreateAndReturnAsync(TGroup groupValue, TItem item)
        {
            var id = await CreateAsync(groupValue, item);
            return await ReadAsync(id, groupValue);
        }

        /// <inheritdoc />
        public abstract Task<PageEnvelope<TItem>> ReadAllAsync(TGroup groupValue, int offset = 0, int? limit = null);

        /// <inheritdoc />
        public abstract Task<TItem> ReadAsync(TId id, TGroup groupValue);

        /// <inheritdoc />
        public abstract Task DeleteAsync(TId id, TGroup groupValue);

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
                taskList.Add(DeleteAsync(identifiable.Id, groupValue));
            }
            await Task.WhenAll(taskList);
        }
    }
}
