using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet
{
    /// <summary>
    /// Tests for testing any storage that implements <see cref="ICrud{TStorable,TId}"/>
    /// </summary>
    [TestClass]
    public abstract class TestICrdBare<TId> : TestICrdBase<TestItemBare, TId>
    {

        /// <summary>
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task Create_Read_Async()
        {
            var initialItem = new TestItemBare();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            var id = await CrdStorage.CreateAsync(initialItem);
            var result = await CrdStorage.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var createdItem = result as TestItemBare;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }

        /// <summary>
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task CreateAndReturn_Async()
        {
            var initialItem = new TestItemBare();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            var result = await CrdStorage.CreateAndReturnAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var createdItem = result as TestItemBare;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }

        /// <summary>
        /// Try to read an item that doesn't exist yet.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FulcrumNotFoundException))]
        public async Task Read_NotFound_Async()
        {
            await CrdStorage.ReadAsync(StorageHelper.CreateNewId<TId>());
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("Expected an exception");
        }

        /// <summary>
        /// Delete an item
        /// </summary>
        [TestMethod]
        public async Task Delete_Async()
        {
            var initialItem = new TestItemBare();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            var id = await CrdStorage.CreateAsync(initialItem);
            await CrdStorage.ReadAsync(id);
            await CrdStorage.DeleteAsync(id);
            try
            {
                await CrdStorage.ReadAsync(id);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("Expected an exception");
            }
            catch (FulcrumNotFoundException)
            {
                // As expected
            }
        }

        /// <summary>
        /// Try to read an item that doesn't exist. Should not result in an exception.
        /// </summary>
        [TestMethod]
        public async Task Delete_NotFound()
        {
            await CrdStorage.DeleteAsync(StorageHelper.CreateNewId<TId>());
        }
    }
}

