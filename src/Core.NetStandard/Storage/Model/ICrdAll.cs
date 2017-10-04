﻿namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// Interface for CR-D operation on any class that implements <see cref="IStorableItem{TId}"/>.
    /// </summary>
    /// <typeparam name="TStorable">The typo of objects that should have CRUD operations.</typeparam>
    /// <typeparam name="TId">The type for the <see cref="IStorableItem{TId}.Id"/> property.</typeparam>
    public interface ICrdAll<TStorable, TId> : ICreate<TStorable, TId>, IRead<TStorable, TId>, IReadAll<TStorable, TId>, IDelete<TId>, IDeleteAll
        where TStorable : IStorableItem<TId>
    {
    }
}
