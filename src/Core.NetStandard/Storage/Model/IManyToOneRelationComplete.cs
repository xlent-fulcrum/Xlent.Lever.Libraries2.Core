using System;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting many-to-many relations.
    /// </summary>
    public interface IManyToOneRelationComplete<TManyModel, TOneModel, TId> : ICrud<TManyModel, TId>, IManyToOneRelation<TManyModel, TOneModel, TId> where TManyModel : class
    {
    }
}
