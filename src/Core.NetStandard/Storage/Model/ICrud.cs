namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Interface for CRUD operation on any class that implements <see cref="IIdentifiable{TId}"/>.
    /// </summary>
    /// <typeparam name="TItem">The typo of objects that should have CRUD operations.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IIdentifiable{TId}.Id"/> property.</typeparam>
    public interface ICrud<TItem, TId> : ICrd<TItem, TId>, IUpdate<TItem, TId>
    {
    }
}
