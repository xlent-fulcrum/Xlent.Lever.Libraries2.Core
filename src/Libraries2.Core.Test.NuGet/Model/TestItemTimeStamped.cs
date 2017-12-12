using System;
using Xlent.Lever.Libraries2.Core.Storage.Model;
#pragma warning disable 659

namespace Xlent.Lever.Libraries2.Core.Test.NuGet.Model
{
    /// <summary>
    /// A minimal storable item that implements <see cref="ITimeStamped"/> to be used in testing
    /// </summary>
    public partial class TestItemTimestamped : TestItemBare, ITimeStamped
    {
        /// <inheritdoc />
        public DateTimeOffset RecordCreatedAt { get; set; }

        /// <inheritdoc />
        public DateTimeOffset RecordUpdatedAt { get; set; }
    }

    #region override object
    public partial class TestItemTimestamped
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is TestItemTimestamped o)) return false;
            if (!base.Equals(obj)) return false;
            return RecordCreatedAt.Equals(o.RecordCreatedAt) && RecordUpdatedAt.Equals(o.RecordUpdatedAt);
        }
    }
    #endregion
}
