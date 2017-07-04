using System;

namespace Xlent.Lever.Libraries2.Core.Storage.Model
{

    /// <summary>
    /// The recommended interfaces for a storable item. Uses a <see cref="Guid"/> as the <see cref="IStorableItem{TId}.Id"/>.
    /// </summary>
    public interface IRecommendedStorableItem : IRecommendedStorableItem<Guid>
    {
    }

    /// <summary>
    /// The recommended interfaces for a storable item.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IRecommendedStorableItem<TId> : IStorableItem<TId>, IOptimisticConcurrencyControlByETag
    {
    }
}
