using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet
{
    /// <summary>
    /// Tests for testing any storage that implements <see cref="ICrud{TStorable,TId}"/>
    /// </summary>
    [TestClass]
    public abstract class TestICrdEtag<TId> : TestICrdBase<TestItemEtag<TId>, TId>
    {
        /// <summary>
        /// Create an item with an id.
        /// </summary>
        [TestMethod]
        public async Task Create_Read_Etag_Async()
        {
            var initialItem = new TestItemEtag<TId>();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(default(TId), initialItem.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(initialItem.Etag);
            var id = await CrdStorage.CreateAsync(initialItem);
            var result = await CrdStorage.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var createdItem = result as TestItemEtag<TId>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(createdItem.Id, default(TId));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem.Etag);
            initialItem.Id = createdItem.Id;
            initialItem.Etag = createdItem.Etag;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }

        /// <summary>
        /// Create an item with an etag.
        /// </summary>
        [TestMethod]
        public async Task CreateAndReturn_Read_Etag_Async()
        {
            var initialItem = new TestItemEtag<TId>();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(default(TId), initialItem.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(initialItem.Etag);
            var result = await CrdStorage.CreateAndReturnAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var createdItem = result as TestItemEtag<TId>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(createdItem.Id, default(TId));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem.Etag);
            initialItem.Id = createdItem.Id;
            initialItem.Etag = createdItem.Etag;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }
    }
}

