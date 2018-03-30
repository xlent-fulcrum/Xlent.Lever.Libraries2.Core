using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Storage
{
    [TestClass]
    public class MemoryManyToOneRecursive : TestIManyToOneRecursive<Guid, Guid?>
    {
        private IManyToOneRelationComplete<TestItemManyToOne<Guid, Guid?>, Guid> _storage;

        [TestInitialize]
        public void Inititalize()
        {
            _storage = new MemoryManyToOneRecursivePersistance<TestItemManyToOne<Guid, Guid?>, Guid>(item => item.ParentId);
        }

        /// <inheritdoc />
        protected override IManyToOneRelationComplete<TestItemManyToOne<Guid, Guid?>, Guid>
            ManyStorageRecursive => _storage;

        /// <inheritdoc />
        protected override IManyToOneRelationComplete<TestItemManyToOne<Guid, Guid?>, Guid>
            ManyStorageNonRecursive => null;

        /// <inheritdoc />
        protected override ICrd<TestItemId<Guid>, Guid> OneStorage => null;
    }
}
