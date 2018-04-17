using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Helpers
{
    /// <summary>
    /// Abstract base class that has a default implementation for 
    /// <see cref="UpdateAndReturnAsync"/> and <see cref="DeleteAllAsync"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class RudBase<TModel, TId> : ReadBase<TModel, TId>, IRud<TModel, TId>
    {
        /// <inheritdoc />
        public abstract Task UpdateAsync(TId id, TModel item, CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task<TModel> UpdateAndReturnAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(item, nameof(item));
            MaybeValidate(item);
            await UpdateAsync(id, item, token);
            return await ReadAsync(id, token);
        }

        /// <inheritdoc />
        public abstract Task DeleteAsync(TId id, CancellationToken token = default(CancellationToken));

        /// <inheritdoc />
        public virtual async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            var errorMessage = $"The method {nameof(DeleteAllAsync)} of the abstract base class {nameof(RudBase<TModel, TId>)} must be overridden when it stores items that are not implementing the interface {nameof(IUniquelyIdentifiable<TId>)}";
            FulcrumAssert.IsTrue(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TModel)), null,
                errorMessage);
            var items = new PageEnvelopeEnumerableAsync<TModel>((offset, t) => ReadAllWithPagingAsync(offset, null, t), token);
            var enumerator = items.GetEnumerator();
            var taskList = new List<Task>();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                var identifiable = item as IUniquelyIdentifiable<TId>;
                FulcrumAssert.IsNotNull(identifiable, null, errorMessage);
                if (identifiable == null) continue;
                taskList.Add(DeleteAsync(identifiable.Id, token));
                if (token.IsCancellationRequested) break;
            }
            await Task.WhenAll(taskList);
        }
    }
}