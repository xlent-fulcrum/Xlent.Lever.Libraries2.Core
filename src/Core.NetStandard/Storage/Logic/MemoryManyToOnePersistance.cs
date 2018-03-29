using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// General class for storing a many to one item in memory.
    /// </summary>
    /// <typeparam name="TManyModel">The model for the children that each points out a parent.</typeparam>
    /// <typeparam name="TOneModel">The model for the parent.</typeparam>
    /// <typeparam name="TId">The type for the id field of the models.</typeparam>
    public class MemoryManyToOnePersistance<TManyModel, TOneModel, TId> : MemoryPersistance<TManyModel, TId>, IManyToOneRelationComplete<TManyModel, TOneModel, TId>
        where TManyModel : class
        where TOneModel : class
    {
        private readonly GetParentIdDelegate _getParentIdDelegate;
        private readonly IRead<TOneModel, TId> _parentReader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getParentIdDelegate">See <see cref="MemoryManyToOneRecursivePersistance{TModel,TId}.GetParentIdDelegate"/>.</param>
        /// <param name="parentReader">Functionality to read a specified parent.</param>
        public MemoryManyToOnePersistance(GetParentIdDelegate getParentIdDelegate, IRead<TOneModel, TId> parentReader)
        {
            InternalContract.RequireNotNull(getParentIdDelegate, nameof(getParentIdDelegate));
            if (typeof(TManyModel) == typeof(TOneModel)) InternalContract.Require(parentReader == null, $"Expected parameter {nameof(parentReader)} to be null when TManyModel and TOneModel are equal.");
            else InternalContract.RequireNotNull(parentReader, nameof(parentReader));
            _getParentIdDelegate = getParentIdDelegate;
            _parentReader = parentReader;
        }

        /// <inheritdoc />
        public async Task<TOneModel> ReadParentAsync(TId childId)
        {
            InternalContract.RequireNotNull(childId, nameof(childId));
            InternalContract.RequireNotDefaultValue(childId, nameof(childId));
            var child = await ReadAsync(childId);
            var parentIdAsObject = _getParentIdDelegate(child);
            if (parentIdAsObject == null) return null;
            if (parentIdAsObject.Equals(default(TId))) return null;
            var parentIdAsId = ConvertToParameterType<TId>(parentIdAsObject);
            return _parentReader == null ? ToOneModel(await ReadAsync(parentIdAsId)) : await _parentReader.ReadAsync(parentIdAsId);
        }

        private TOneModel ToOneModel(TManyModel manyModel)
        {
            InternalContract.RequireNotNull(manyModel, nameof(manyModel));
            var oneModel = manyModel as TOneModel;
            FulcrumAssert.IsNotNull(oneModel);
            return oneModel;
        }

        /// <summary>
        /// A delegate method for getting the parent id from a stored item.
        /// </summary>
        /// <param name="item">The item to get the parent for.</param>
        public delegate object GetParentIdDelegate(TManyModel item);

        /// <inheritdoc />
        public Task<PageEnvelope<TManyModel>> ReadChildrenWithPagingAsync(TId parentId, int offset = 0, int? limit = null)
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
                var page = new PageEnvelope<TManyModel>(offset, limit.Value, MemoryItems.Count, list);
                return Task.FromResult(page);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TManyModel>> ReadChildrenAsync(TId parentId, int limit = int.MaxValue)
        {
            InternalContract.RequireNotNull(parentId, nameof(parentId));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));


            var result = new List<TManyModel>();
            var offset = 0;
            while (true)
            {
                var page = await ReadChildrenWithPagingAsync(parentId, offset);
                if (page.PageInfo.Returned == 0) break;
                result.AddRange(page.Data);
                offset += page.PageInfo.Returned;
            }

            return result;
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(TId parentId)
        {
            InternalContract.RequireNotNull(parentId, nameof(parentId));
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            var errorMessage = $"{nameof(TManyModel)} must implement the interface {nameof(IUniquelyIdentifiable<TId>)} for this method to work.";
            InternalContract.Require(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TManyModel)), errorMessage);
            var items = new PageEnvelopeEnumerableAsync<TManyModel>(o => ReadChildrenWithPagingAsync(parentId, o));
            var enumerator = items.GetEnumerator();
            while (await enumerator.MoveNextAsync())
            {
                if (!(enumerator.Current is IUniquelyIdentifiable<TId> item)) continue;
                await DeleteAsync(item.Id);
            }
        }
    }
}
