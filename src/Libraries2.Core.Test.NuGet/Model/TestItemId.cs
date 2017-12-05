using Xlent.Lever.Libraries2.Core.Misc.Models;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet.Model
{
    /// <summary>
    /// A minimal storable item to be used in testing
    /// </summary>
    public partial class TestItemId<TId> : TestItemBare, IIdentifiable<TId>
    {
        /// <inheritdoc />
        public TId Id { get; set; }
    }

    public partial class TestItemId<TId> : IDeepCopy<TestItemId<TId>>
    {
        public void DeepCopy(TestItemId<TId> source)
        {
            base.DeepCopy(source);
            Id = source.Id;
        }

        /// <inheritdoc />
        public new TestItemId<TId> DeepCopy()
        {
            return StorageHelper.DeepCopy(this);
        }

        public new void DeepCopy(IItemForTesting source)
        {
            var x = source as TestItemId<TId>;
            DeepCopy(x);
        }

        IItemForTesting IDeepCopy<IItemForTesting>.DeepCopy()
        {
            return DeepCopy();
        }
    }

    #region override object
    public partial class TestItemId<TId>
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is TestItemId<TId> o)) return false;
            return Id.Equals(o.Id) && base.Equals(obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Id.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Value} ({Id})";
        }
    }
    #endregion
}
