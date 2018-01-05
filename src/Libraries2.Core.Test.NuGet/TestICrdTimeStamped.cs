using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet
{
    /// <summary>
    /// Tests for testing any storage that implements <see cref="ICrud{TestItemTimestamped,TId}"/>
    /// </summary>
    [TestClass]
    public abstract class TestICrdTimeStamped<TId> : TestICrdBase<TestItemTimestamped<TId>, TId>
    {
        /// <summary>
        /// Create an item with an id.
        /// </summary>
        [TestMethod]
        public async Task Create_Read_Etag_Async()
        {
            var initialItem = new TestItemTimestamped<TId>();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(default(TId), initialItem.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(initialItem.RecordCreatedAt == default(DateTimeOffset));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(initialItem.RecordUpdatedAt == default(DateTimeOffset));
            var id = await CrdStorage.CreateAsync(initialItem);
            var createdItem = await CrdStorage.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(createdItem.Id, default(TId));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(createdItem.RecordCreatedAt != default(DateTimeOffset));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(createdItem.RecordUpdatedAt != default(DateTimeOffset));
            initialItem.Id = createdItem.Id;
            initialItem.RecordCreatedAt = createdItem.RecordCreatedAt;
            initialItem.RecordUpdatedAt = createdItem.RecordUpdatedAt;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }

        /// <summary>
        /// Create an item with an etag.
        /// </summary>
        [TestMethod]
        public async Task CreateAndReturn_Read_Etag_Async()
        {
            var initialItem = new TestItemTimestamped<TId>();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(default(TId), initialItem.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(initialItem.RecordCreatedAt == default(DateTimeOffset));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(initialItem.RecordUpdatedAt == default(DateTimeOffset));
            var createdItem = await CrdStorage.CreateAndReturnAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(createdItem.Id, default(TId));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(createdItem.RecordCreatedAt != default(DateTimeOffset));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(createdItem.RecordUpdatedAt != default(DateTimeOffset));
            initialItem.Id = createdItem.Id;
            initialItem.RecordCreatedAt = createdItem.RecordCreatedAt;
            initialItem.RecordUpdatedAt = createdItem.RecordUpdatedAt;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }
    }
}

