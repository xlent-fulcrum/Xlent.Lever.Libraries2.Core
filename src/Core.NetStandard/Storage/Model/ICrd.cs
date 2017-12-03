namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Interface for CR-D operation on any class that implements <see cref="IIdentifiable{TId}"/>.
    /// </summary>
    /// <typeparam name="TItem">The typo of objects that should have CRUD operations.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IIdentifiable{TId}.Id"/> property.</typeparam>
    public interface ICrd<TItem, TId> : ICreate<TItem, TId>, IRead<TItem, TId>, IDelete<TId>, IReadAll<TItem>, IDeleteAll
    {
    }
}
