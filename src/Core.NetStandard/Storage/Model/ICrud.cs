namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Interface for CRUD operations."/>.
    /// </summary>
    /// <typeparam name="TItem">The typ of objects that should have CRUD operations.</typeparam>
    /// <typeparam name="TId">The type for the id.</typeparam>
    public interface ICrud<TItem, TId> : ICrd<TItem, TId>, IUpdate<TItem, TId>
    {
    }
}
