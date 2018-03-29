using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Storage
{
    [TestClass]
    public class MemoryManyToOne : TestIManyToOne<Guid, Guid?>
    {
        private ICrud<TestItemId<Guid>, Guid> _oneStorage;
        private IManyToOneRelationComplete<TestItemManyToOne<Guid, Guid?>, TestItemId<Guid>, Guid> _manyStorage;

        [TestInitialize]
        public void Inititalize()
        {
            _oneStorage = new MemoryPersistance<TestItemId<Guid>, Guid>();
            _manyStorage = new MemoryManyToOnePersistance<TestItemManyToOne<Guid, Guid?>, TestItemId<Guid>, Guid>(item => item.ParentId, _oneStorage);
        }

        /// <inheritdoc />
        protected override IManyToOneRelationComplete<TestItemManyToOne<Guid, Guid?>, TestItemManyToOne<Guid, Guid?>, Guid>
            ManyStorageRecursive => null;

        /// <inheritdoc />
        protected override IManyToOneRelationComplete<TestItemManyToOne<Guid, Guid?>, TestItemId<Guid>, Guid>
            ManyStorageNonRecursive => _manyStorage;

        /// <inheritdoc />
        protected override ICrd<TestItemId<Guid>, Guid> OneStorage => _oneStorage;
    }
}
