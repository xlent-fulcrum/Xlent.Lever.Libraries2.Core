namespace Xlent.Lever.Libraries2.Core.Crud.Interfaces
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    /// <typeparam name="TManyToManyModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TReferenceModel1"></typeparam>
    /// <typeparam name="TReferenceModel2"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IManyToManyComplete<TManyToManyModel, TReferenceModel1, TReferenceModel2, TId> :
        IManyToManyComplete<TManyToManyModel, TManyToManyModel, TReferenceModel1, TReferenceModel2, TId>
    {
    }
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    /// <typeparam name="TManyToManyModelCreate">The type for creating objects in persistant storage.</typeparam>
    /// <typeparam name="TManyToManyModel">The type of objects that are returned from persistant storage.</typeparam>
    /// <typeparam name="TReferenceModel1"></typeparam>
    /// <typeparam name="TReferenceModel2"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IManyToManyComplete<in TManyToManyModelCreate, TManyToManyModel, TReferenceModel1, TReferenceModel2, TId> :
        IManyToMany<TReferenceModel1, TReferenceModel2, TId>,
        IManyToManyBiased1Complete<TManyToManyModelCreate, TManyToManyModel, TReferenceModel2, TId>,
        IManyToManyBiased2Complete<TManyToManyModelCreate, TManyToManyModel, TReferenceModel1, TId> where TManyToManyModel : TManyToManyModelCreate
    {
    }
}
