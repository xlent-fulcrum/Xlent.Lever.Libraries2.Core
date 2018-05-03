namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Interface for CRUD operations."/>.
    /// </summary>
    /// <typeparam name="TModel">The type for the objects in persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the id.</typeparam>
    public interface ICrud<TModel, TId> : ICrud<TModel, TModel, TId>, ICrd<TModel, TId>
    {
    }

    /// <summary>
    /// Interface for CRUD operations."/>.
    /// </summary>
    /// <typeparam name="TModelCreate">The type for creating objects in persistant storage.</typeparam>
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the id.</typeparam>
    public interface ICrud<in TModelCreate, TModel, TId> : ICrd<TModelCreate, TModel, TId>, IRud<TModel, TId>
        where TModel : TModelCreate
    {
    }
}
