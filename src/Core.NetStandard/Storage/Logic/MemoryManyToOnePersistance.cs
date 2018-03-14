using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class MemoryManyToOnePersistance<TManyModel, TOneModel, TId> : MemoryPersistance<TManyModel, TId>, IManyToOneRelationComplete<TManyModel, TOneModel, TId> where TManyModel : class
    {
        private readonly GetParentIdDelegate _getParentIdDelegate;
        private readonly IRead<TOneModel, TId> _parentHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getParentIdDelegate">See <see cref="GetParentIdDelegate"/>.</param>
        /// <param name="parentHandler">Functionality to read a specified parent.</param>
        public MemoryManyToOnePersistance(GetParentIdDelegate getParentIdDelegate, IRead<TOneModel, TId> parentHandler)
        {
            _getParentIdDelegate = getParentIdDelegate;
            _parentHandler = parentHandler;
        }

        /// <summary>
        /// A delegate method for getting the parent id from a stored item.
        /// </summary>
        /// <param name="item">The item to get the parent for.</param>
        public delegate TId GetParentIdDelegate(TManyModel item);

        /// <inheritdoc />
        public Task<PageEnvelope<TManyModel>> ReadChildrenAsync(TId parentId, int offset = 0, int? limit = null)
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
        public async Task<TOneModel> ReadParentAsync(TId childId)
        {
            var child = await ReadAsync(childId);
            return await _parentHandler.ReadAsync(_getParentIdDelegate(child));
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(TId parentId)
        {
            InternalContract.RequireNotNull(parentId, nameof(parentId));
            var errorMessage = $"{nameof(TManyModel)} must implement the interface {nameof(IUniquelyIdentifiable<TId>)} for this method to work.";
            InternalContract.Require(typeof(IUniquelyIdentifiable<TId>).IsAssignableFrom(typeof(TManyModel)), errorMessage);
            var items = new PageEnvelopeEnumerableAsync<TManyModel>((o) => ReadAllAsync(o));
            var enumerator = items.GetEnumerator();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current as IUniquelyIdentifiable<TId>;
                if (item == null) continue;
                await DeleteAsync(item.Id);
            }
        }
    }
}
