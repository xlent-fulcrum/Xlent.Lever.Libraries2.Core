namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Interface for -RUD operations."/>.
    /// </summary>
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the id.</typeparam>
    public interface IRud<TModel, in TId> : IReadAll<TModel, TId>, IUpdate<TModel, TId>, IDeleteAll<TId>
    {
    }
}
