using Xlent.Lever.Libraries2.Core.Storage.Model;
#pragma warning disable 659

namespace Xlent.Lever.Libraries2.Core.Test.NuGet.Model
{
    /// <summary>
    /// A minimal storable item that implements <see cref="IOptimisticConcurrencyControlByETag"/> to be used in testing
    /// </summary>
    public partial class TestItemEtag : TestItemBare, IOptimisticConcurrencyControlByETag
    {
        /// <inheritdoc />
        public string Etag { get; set; }
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
