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
    public abstract class TestICrudBare<TId> : TestICrdBare<TId>
    {
        protected override ICrd<TestItemBare, TId> CrdStorage => CrudStorage;

        /// <summary>
        /// Try to create an item that is not valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FulcrumNotFoundException))]
        public async Task Update_NotFound_Async()
        {
            var updateItem = new TestItemBare();
            updateItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            await CrudStorage.UpdateAsync(StorageHelper.CreateNewId<TId>(), updateItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("Expected an exception");
        }

        /// <summary>
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task Update_Read_Bare_Async()
        {
            var id = await CreateItemAsync(TypeOfTestDataEnum.Variant1);
            var updatedItem = await UpdateItemAsync(id, TypeOfTestDataEnum.Variant2);
            var readItem = await ReadItemAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(updatedItem, readItem);
        }
    }
}

