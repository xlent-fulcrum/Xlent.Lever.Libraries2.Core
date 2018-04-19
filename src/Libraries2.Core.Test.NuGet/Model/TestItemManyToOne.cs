using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet.Model
{
    /// <summary>
    /// A minimal storable item that implements <see cref="IUniquelyIdentifiable{TId}"/> to be used in testing
    /// </summary>
    public class TestItemManyToOne<TId, TReferenceId> : TestItemManyToOneCreate<TReferenceId>, IUniquelyIdentifiable<TId>
    {
        /// <inheritdoc />
        public TId Id { get; set; }
    }
}
