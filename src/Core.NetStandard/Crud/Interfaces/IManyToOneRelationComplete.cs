namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    /// <typeparam name="TManyModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IManyToOneRelationComplete<TManyModel, TId> : IManyToOneRelationComplete<TManyModel, TManyModel, TId>, ICrud<TManyModel, TId>, ICrudWithSpecifiedId<TManyModel, TId>
    {
    }

    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    /// <typeparam name="TManyModelCreate">The type for creating objects in persistant storage.</typeparam>
    /// <typeparam name="TManyModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IManyToOneRelationComplete<in TManyModelCreate, TManyModel, TId> : ICrud<TManyModelCreate, TManyModel, TId>, ICrudWithSpecifiedId<TManyModelCreate, TManyModel, TId>, IManyToOneRelation<TManyModel, TId> where TManyModel : TManyModelCreate
    {
    }
}
