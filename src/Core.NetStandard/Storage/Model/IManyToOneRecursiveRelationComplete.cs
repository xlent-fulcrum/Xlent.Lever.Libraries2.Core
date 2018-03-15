namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting a recursive many-to-many relations within the same model.
    /// </summary>
    public interface IManyToOneRecursiveRelationComplete<TModel, TId, in TReferenceId> : ICrud<TModel, TId>, IManyToOneRecursiveRelation<TModel, TId, TReferenceId> 
        where TModel : class
        where TReferenceId : TId
    {
    }
}
