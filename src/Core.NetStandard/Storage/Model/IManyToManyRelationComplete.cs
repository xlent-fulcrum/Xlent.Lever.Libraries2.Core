using System;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    public interface IManyToManyRelationComplete<TManyToManyModel, TReferenceModel1, TReferenceModel2, TId> : ICrud<TManyToManyModel, TId>, IManyToManyRelation<TReferenceModel1, TReferenceModel2, TId> where TManyToManyModel : class
    {
    }
}
