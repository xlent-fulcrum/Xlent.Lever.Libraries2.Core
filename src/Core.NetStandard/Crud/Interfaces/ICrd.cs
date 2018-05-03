using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Interface for CR-D operation on any class that implements <see cref="IUniquelyIdentifiable{TId}"/>.
    /// </summary>
    /// <typeparam name="TModel">The type for the objects in persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IUniquelyIdentifiable{TId}.Id"/> property.</typeparam>
    public interface ICrd<TModel, TId> : ICrd<TModel, TModel, TId>, ICreate<TModel, TId>, ICreateWithSpecifiedId<TModel, TId>
    {
    }

    /// <summary>
    /// Interface for CR-D operation on any class that implements <see cref="IUniquelyIdentifiable{TId}"/>.
    /// </summary>
    /// <typeparam name="TModelCreate">The type for creating objects in persistant storage.</typeparam>
    /// <typeparam name="TModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IUniquelyIdentifiable{TId}.Id"/> property.</typeparam>
    public interface ICrd<in TModelCreate, TModel, TId> : ICreate<TModelCreate, TModel, TId>, ICreateWithSpecifiedId<TModelCreate, TModel, TId>, IRead<TModel, TId>, IDelete<TId>, ILockable<TId>
        where TModel : TModelCreate
    {
    }
}
