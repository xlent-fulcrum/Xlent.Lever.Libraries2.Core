﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.MemoryStorage;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Storage
{
    [TestClass]
    public class MemoryManyToOneTest : TestIManyToOne<Guid, Guid?>
    {
        private ICrud<TestItemBare, TestItemId<Guid>, Guid> _oneStorage;
        private IManyToOneRelationComplete<TestItemManyToOneCreate<Guid?>, TestItemManyToOne<Guid, Guid?>, Guid> _manyStorage;

        [TestInitialize]
        public void Inititalize()
        {
            _oneStorage = new CrudMemory<TestItemBare, TestItemId<Guid>, Guid>();
            _manyStorage = new ManyToOneMemory<TestItemManyToOneCreate<Guid?>, TestItemManyToOne<Guid, Guid?>, Guid>(item => item.ParentId);
        }

        /// <inheritdoc />
        protected override IManyToOneRelationComplete<TestItemManyToOneCreate<Guid?>, TestItemManyToOne<Guid, Guid?>, Guid>
            ManyStorageRecursive => null;

        /// <inheritdoc />
        protected override IManyToOneRelationComplete<TestItemManyToOneCreate<Guid?>, TestItemManyToOne<Guid, Guid?>, Guid>
            ManyStorageNonRecursive => _manyStorage;

        /// <inheritdoc />
        protected override ICrd<TestItemBare, TestItemId<Guid>, Guid> OneStorage => _oneStorage;
    }
}
