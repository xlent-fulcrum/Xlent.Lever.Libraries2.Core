﻿namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Functionality for persisting a recursive many-to-many relations within the same model.
    /// </summary>
    public interface IManyToOneRecursiveRelationComplete<TModel, TId> : ICrud<TModel, TId>, IManyToOneRecursiveRelation<TModel, TId> 
        where TModel : class
    {
    }
}