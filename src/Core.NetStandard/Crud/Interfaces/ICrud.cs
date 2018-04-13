namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Interface for CRUD operations."/>.
    /// </summary>
    /// <typeparam name="TModel">The typ of objects that should have CRUD operations.</typeparam>
    /// <typeparam name="TId">The type for the id.</typeparam>
    public interface ICrud<TModel, TId> : ICrd<TModel, TId>, IUpdate<TModel, TId>
    {
    }
}
