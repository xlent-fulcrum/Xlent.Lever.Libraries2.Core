namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Interface for CR-D operation on any class that implements <see cref="IStorableItem{TId}"/>.
    /// </summary>
    /// <typeparam name="TStorable">The typo of objects that should have CRUD operations.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IStorableItem{TId}.Id"/> property.</typeparam>
    public interface ICrd<TStorable, in TId> : ICreate<TStorable, TId>, IRead<TStorable, TId>, IDelete<TId>
        where TStorable : IStorableItem<TId>
    {
    }
}
