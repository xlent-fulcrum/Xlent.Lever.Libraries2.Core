using System;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Mapping
{
    /// <summary>
    /// Help methods for mapping
    /// </summary>
    public static class MapperHelper
    {
        /// <summary>
        /// Map an id between two types.
        /// </summary>
        /// <param name="sourceId">The id to map.</param>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <exception cref="FulcrumNotImplementedException">Thrown if the type was not recognized. Please add that type to the class <see cref="MapperHelper"/>.</exception>
        public static TTarget MapId<TTarget, TSource>(TSource sourceId)
        {
            if (sourceId == null) return default(TTarget);
            if (Equals(sourceId, default(TSource))) return default(TTarget);
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            if (targetType == typeof(string))
            {
                return (TTarget)(object)sourceId.ToString();
            }
            if (targetType == typeof(Guid))
            {
                var success = Guid.TryParse(sourceId.ToString(), out Guid targetId);
                InternalContract.Require(success, $"Could not parse parameter {nameof(sourceId)} ({sourceId}) of type {sourceType.Name} into type Guid.");
                return (TTarget)(object)targetId;
            }
            throw new FulcrumNotImplementedException($"There is currently no rule on how to convert an id from type {sourceType.Name} to type {targetType.Name}.");
        }

    }
}
