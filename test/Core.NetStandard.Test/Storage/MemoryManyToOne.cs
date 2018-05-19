using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.MemoryStorage;
using Xlent.Lever.Libraries2.Core.Test.NuGet.ManyToOne;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Storage
{
    [TestClass]
    public class MemoryManyToOneTest : TestIManyToOne<Guid, Guid?>
    {
        private ICrud<TestItemId<Guid>, Guid> _oneStorage;
        private IManyToOneCrud<TestItemManyToOneCreate<Guid?>, TestItemManyToOne<Guid, Guid?>, Guid> _manyStorage;

        [TestInitialize]
        public void Inititalize()
        {
            _oneStorage = new CrudMemory<TestItemId<Guid>, Guid>();
            _manyStorage = new ManyToOneMemory<TestItemManyToOneCreate<Guid?>, TestItemManyToOne<Guid, Guid?>, Guid>(item => item.ParentId);
        }

        /// <inheritdoc />
        protected override IManyToOneCrud<TestItemManyToOneCreate<Guid?>, TestItemManyToOne<Guid, Guid?>, Guid>
            ManyStorageRecursive => null;

        /// <inheritdoc />
        protected override IManyToOneCrud<TestItemManyToOneCreate<Guid?>, TestItemManyToOne<Guid, Guid?>, Guid>
            ManyStorageNonRecursive => _manyStorage;

        /// <inheritdoc />
        protected override ICrd<TestItemId<Guid>, Guid> OneStorage => _oneStorage;
    }
}
