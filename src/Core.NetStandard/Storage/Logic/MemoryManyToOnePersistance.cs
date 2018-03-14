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
    public class MemoryManyToOnePersistance<TManyModel, TOneModel, TId> : MemoryManyToOneRecursivePersistance<TManyModel, TId>, IManyToOneRelationComplete<TManyModel, TOneModel, TId> where TManyModel : class
    {
        private readonly GetParentIdDelegate _getParentIdDelegate;
        private readonly IRead<TOneModel, TId> _parentHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getParentIdDelegate">See <see cref="MemoryManyToOneRecursivePersistance{TModel,TId}.GetParentIdDelegate"/>.</param>
        /// <param name="parentHandler">Functionality to read a specified parent.</param>
        public MemoryManyToOnePersistance(GetParentIdDelegate getParentIdDelegate, IRead<TOneModel, TId> parentHandler = null)
        :base(getParentIdDelegate)
        {
            _getParentIdDelegate = getParentIdDelegate;
            _parentHandler = parentHandler;
        }

        /// <inheritdoc />
        public new async Task<TOneModel> ReadParentAsync(TId childId)
        {
            var child = await ReadAsync(childId);
            return await _parentHandler.ReadAsync(_getParentIdDelegate(child));
        }
    }
}
