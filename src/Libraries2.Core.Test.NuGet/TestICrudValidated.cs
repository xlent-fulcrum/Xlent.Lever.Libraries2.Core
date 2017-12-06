using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public abstract class TestICrudValidated<TId> : TestICrdValidated<TId>
    {
        protected override ICrd<TestItemValidated, TId> CrdStorage => CrudStorage;

        /// <summary>
        /// Try to create an item that is not valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FulcrumContractException))]
        public async Task Update_ValidationFailed_Async()
        {
            var id = await CreateItemAsync(TypeOfTestDataEnum.Default);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(default(TId), id);
            var updatedItem = new TestItemValidated();
            updatedItem.InitializeWithDataForTesting(TypeOfTestDataEnum.ValidationFail);
            await CrudStorage.UpdateAsync(id, updatedItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail($"Expected the method {nameof(CrudStorage.UpdateAsync)} to detect that the data was not valid and throw the exception {nameof(FulcrumContractException)}.");
        }

        /// <summary>
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task Update_Read_Validated_Async()
        {
            var id = await CreateItemAsync(TypeOfTestDataEnum.Variant1);
            var updatedItem = await UpdateItemAsync(id, TypeOfTestDataEnum.Variant2);
            var readItem = await ReadItemAsync(id);
            readItem.Validate(null);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(updatedItem, readItem);
        }
    }
}

