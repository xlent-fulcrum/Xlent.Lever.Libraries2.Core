namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// General class for storing a many to one item in memory.
    /// </summary>
    /// <typeparam name="TModel">The model for the parent.</typeparam>
    /// <typeparam name="TId">The type for the id field of the models.</typeparam>
    public class MemoryManyToOneRecursivePersistance<TModel, TId> : MemoryManyToOnePersistance<TModel, TId> 
        where TModel : class
        
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getParentIdDelegate">See <see cref="MemoryManyToOnePersistance{TManyMode,TId}.GetParentIdDelegate"/>.</param>
        public MemoryManyToOneRecursivePersistance(GetParentIdDelegate getParentIdDelegate)
        :base(getParentIdDelegate)
        {
        }
    }
}
