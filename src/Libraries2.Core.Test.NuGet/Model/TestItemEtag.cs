using Xlent.Lever.Libraries2.Core.Misc.Models;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
#pragma warning disable 659

namespace Xlent.Lever.Libraries2.Core.Test.NuGet.Model
{
    /// <summary>
    /// A minimal storable item to be used in testing
    /// </summary>
    public partial class TestItemEtag : TestItemBare, IOptimisticConcurrencyControlByETag
    {
        /// <inheritdoc />
        public string Etag { get; set; }
    }

    public partial class TestItemEtag : IDeepCopy<TestItemEtag>
    {
        /// <inheritdoc />
        public void DeepCopy(TestItemEtag source)
        {
            base.DeepCopy(source);
            Etag = source.Etag;
        }

        /// <inheritdoc />
        public new TestItemEtag DeepCopy()
        {
            return StorageHelper.DeepCopy(this);
        }

        public new void DeepCopy(IItemForTesting source)
        {
            var x = source as TestItemEtag;
            DeepCopy(x);
        }
    }

    #region override object
    public partial class TestItemEtag
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is TestItemEtag o)) return false;
            return Etag.Equals(o.Etag) && base.Equals(obj);
        }
    }
    #endregion
}
