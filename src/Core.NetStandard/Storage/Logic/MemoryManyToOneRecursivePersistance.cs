using System.Linq;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// General class for storing a many to one item in memory.
    /// </summary>
    /// <typeparam name="TModel">The model for the parent.</typeparam>
    /// <typeparam name="TId">The type for the id field of the models.</typeparam>
    /// <typeparam name="TReferenceId">The type for the reference field of the model.</typeparam>
    public class MemoryManyToOneRecursivePersistance<TModel, TId, TReferenceId> : MemoryPersistance<TModel, TId>, IManyToOneRecursiveRelationComplete<TModel, TId, TReferenceId> 
        where TModel : class
        
    {
        private readonly GetParentIdDelegate _getParentIdDelegate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getParentIdDelegate">See <see cref="GetParentIdDelegate"/>.</param>
        public MemoryManyToOneRecursivePersistance(GetParentIdDelegate getParentIdDelegate)
        {
            InternalContract.RequireNotNull(getParentIdDelegate, nameof(getParentIdDelegate));
            _getParentIdDelegate = getParentIdDelegate;
        }

        /// <summary>
        /// A delegate method for getting the parent id from a stored item.
        /// </summary>
        /// <param name="item">The item to get the parent for.</param>
        public delegate TReferenceId GetParentIdDelegate(TModel item);

        /// <inheritdoc />
        public Task<PageEnvelope<TModel>> ReadChildrenAsync(TReferenceId parentId, int offset = 0, int? limit = null)
        {
            limit = limit ?? PageInfo.DefaultLimit;
            InternalContract.RequireNotNull(parentId, nameof(parentId));
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            lock (MemoryItems)
            {
                var list = MemoryItems.Values
                    .Where(i => parentId.Equals(_getParentIdDelegate(i)))
                    .Skip(offset)
                    .Take(limit.Value);
                var page = new PageEnvelope<TModel>(offset, limit.Value, MemoryItems.Count, list);
                return Task.FromResult(page);
            }
        }

        /// <inheritdoc />
        public async Task<TModel> ReadParentAsync(TId childId)
        {
            InternalContract.RequireNotNull(childId, nameof(childId));
            InternalContract.RequireNotDefaultValue(childId, nameof(childId));
            var child = await ReadAsync(childId);
            var parentIdAsReference = _getParentIdDelegate(child);
            if (parentIdAsReference == null) return null;
            if (parentIdAsReference.Equals(default(TReferenceId))) return null;
            var parentIdAsId = ConvertBetweenParameterTypes<TId, TReferenceId>(parentIdAsReference);

            return await ReadAsync(parentIdAsId);
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(TReferenceId parentId)
        {
            InternalContract.RequireNotNull(parentId, nameof(parentId));
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            var errorMessage = $"{nameof(TModel)} must implement the interface {nameof(IUniquelyIdentifiable<TId>)} for this method to work.";
            InternalContract.Require(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TModel)), errorMessage);
            var items = new PageEnvelopeEnumerableAsync<TModel>((o) => ReadAllAsync(o));
            var enumerator = items.GetEnumerator();
            while (await enumerator.MoveNextAsync())
            {
                if (!(enumerator.Current is IUniquelyIdentifiable<TId> item)) continue;
                await DeleteAsync(item.Id);
            }
        }
    }
}
