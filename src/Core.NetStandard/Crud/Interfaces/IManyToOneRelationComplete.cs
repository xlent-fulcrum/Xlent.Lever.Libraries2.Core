namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    public interface IManyToOneRelationComplete<TManyModel, TId> : ICrud<TManyModel, TId>, IManyToOneRelation<TManyModel, TId>
    {
    }
}
