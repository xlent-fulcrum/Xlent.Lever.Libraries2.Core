namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    /// <typeparam name="TManyModelCreate">The type for creating objects in persistant storage.</typeparam>
    /// <typeparam name="TManyModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IManyToOneRelationComplete<in TManyModelCreate, TManyModel, TId> : ICrud<TManyModelCreate, TManyModel, TId>, IManyToOneRelation<TManyModel, TId>
    {
    }
}
