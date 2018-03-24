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
    public class MemoryManyToOnePersistance<TManyModel, TOneModel, TId> : MemoryManyToOneRecursivePersistance<TManyModel, TId>, IManyToOneRelationComplete<TManyModel, TOneModel, TId>
        where TManyModel : class
        where TOneModel : class
    {
        private readonly GetParentIdDelegate _getParentIdDelegate;
        private readonly IRead<TOneModel, TId> _parentHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getParentIdDelegate">See <see cref="MemoryManyToOneRecursivePersistance{TModel,TId}.GetParentIdDelegate"/>.</param>
        /// <param name="parentHandler">Functionality to read a specified parent.</param>
        public MemoryManyToOnePersistance(GetParentIdDelegate getParentIdDelegate, IRead<TOneModel, TId> parentHandler)
        :base(getParentIdDelegate)
        {
            InternalContract.RequireNotNull(getParentIdDelegate, nameof(getParentIdDelegate));
            InternalContract.RequireNotNull(parentHandler, nameof(parentHandler));
            _getParentIdDelegate = getParentIdDelegate;
            _parentHandler = parentHandler;
        }

        /// <inheritdoc />
        public new async Task<TOneModel> ReadParentAsync(TId childId)
        {
            InternalContract.RequireNotNull(childId, nameof(childId));
            InternalContract.RequireNotDefaultValue(childId, nameof(childId));
            var child = await ReadAsync(childId);
            var parentIdAsObject = _getParentIdDelegate(child);
            if (parentIdAsObject == null) return null;
            if (parentIdAsObject.Equals(default(TId))) return null;
            var parentIdAsId = ConvertToParameterType<TId>(parentIdAsObject);
            return await _parentHandler.ReadAsync(parentIdAsId);
        }
    }
}
