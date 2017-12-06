﻿using System;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{
    /// <summary>
    /// A convenience class that implements the recommended interfaces for a storable item, using a Guid as <see cref="IUniquelyIdentifiable{TId}.Id"/>.
    /// </summary>
    public abstract class StorableItem : StorableItem<Guid>
    {
    }

    /// <summary>
    /// A convenience class that implements the recommended interfaces for a storable item.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class StorableItem<TId> : IRecommendedStorableItem<TId>
    {
        /// <inheritdoc />
        public TId Id { get; set; }

        /// <inheritdoc />
        public virtual string Etag { get; set; }

        /// <inheritdoc />
        public abstract void Validate(string errorLocation, string propertyPath = "");
    }
}
