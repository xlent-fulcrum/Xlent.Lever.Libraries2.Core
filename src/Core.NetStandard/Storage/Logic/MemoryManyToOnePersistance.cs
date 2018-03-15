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
    /// <typeparam name="TReferenceId">The type for the reference field of the model.</typeparam>
    public class MemoryManyToOnePersistance<TManyModel, TOneModel, TId, TReferenceId> : MemoryManyToOneRecursivePersistance<TManyModel, TId, TReferenceId>, IManyToOneRelationComplete<TManyModel, TOneModel, TId, TReferenceId>
        where TManyModel : class
        where TOneModel : class
    {
        private readonly GetParentIdDelegate _getParentIdDelegate;
        private readonly IRead<TOneModel, TId> _parentHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getParentIdDelegate">See <see cref="MemoryManyToOneRecursivePersistance{TModel,TId,TReferenceId}.GetParentIdDelegate"/>.</param>
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
            var parentIdAsReference = _getParentIdDelegate(child);
            if (parentIdAsReference == null) return null;
            if (parentIdAsReference.Equals(default(TReferenceId))) return null;
            var parentIdAsId = ConvertBetweenParameterTypes<TId, TReferenceId>(parentIdAsReference);
            return await _parentHandler.ReadAsync(parentIdAsId);
        }
    }
}
