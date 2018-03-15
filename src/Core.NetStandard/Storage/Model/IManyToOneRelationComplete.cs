namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    public interface IManyToOneRelationComplete<TManyModel, TOneModel, TId, in TReferenceId> : ICrud<TManyModel, TId>, IManyToOneRelation<TManyModel, TOneModel, TId, TReferenceId> 
        where TManyModel : class
        where TReferenceId : TId
    {
    }
}
