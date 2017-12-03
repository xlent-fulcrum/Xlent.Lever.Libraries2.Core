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
    public abstract class TestICrud<TId> : TestICrd<TId>
    {
        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICrud<IItemForTesting, TId> CrudStorage { get; }

        protected override ICrd<IItemForTesting, TId> CrdStorage => CrudStorage;

        #region CreateAsync_ReadAsync

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
        /// Try to create an item that is not valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FulcrumContractException))]
        public async Task Update_ValidationFailed_Async()
        {
            var id = await CreateItemAsync<TestItemValidated>(TypeOfTestDataEnum.Default);
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
        public async Task Update_Read_Bare_Async()
        {
            var id = await CreateItemAsync<TestItemBare>(TypeOfTestDataEnum.Variant1);
            var updatedItem = await UpdateItemAsync<TestItemBare>(id, TypeOfTestDataEnum.Variant2);
            var readItem = await ReadItemAsync<TestItemBare>(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(updatedItem, readItem);
        }

        /// <summary>
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task Update_Read_Validated_Async()
        {
            var id = await CreateItemAsync<TestItemValidated>(TypeOfTestDataEnum.Variant1);
            var updatedItem = await UpdateItemAsync<TestItemValidated>(id, TypeOfTestDataEnum.Variant2);
            var readItem = await ReadItemAsync<TestItemValidated>(id);
            readItem.Validate(null);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(updatedItem, readItem);
        }

        /// <summary>
        /// Create an item with an id.
        /// </summary>
        [TestMethod]
        public async Task Update_Read_Id_Async()
        {
            var id = await CreateItemAsync<TestItemId<TId>>(TypeOfTestDataEnum.Variant1);
            var updatedItem = await UpdateItemAsync<TestItemId<TId>>(id, TypeOfTestDataEnum.Variant2);
            var readItem = await ReadItemAsync<TestItemId<TId>>(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(updatedItem, readItem);
            updatedItem.Id = readItem.Id;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(updatedItem, readItem);
        }

        /// <summary>
        /// Create an item with an id.
        /// </summary>
        [TestMethod]
        public async Task Update_Read_Etag_Async()
        {
            var id = await CreateItemAsync<TestItemEtag>(TypeOfTestDataEnum.Variant1);
            var updatedItem = await UpdateItemAsync<TestItemEtag>(id, TypeOfTestDataEnum.Variant2);
            var readItem = await ReadItemAsync<TestItemEtag>(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(updatedItem, readItem);
            updatedItem.Etag = readItem.Etag;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(updatedItem, readItem);
        }

        #endregion

        private async Task<TId> CreateItemAsync<T>(TypeOfTestDataEnum type) 
            where T : IItemForTesting, new()
        {
            var initialItem = new T();
            initialItem.InitializeWithDataForTesting(type);
            var id = await CrudStorage.CreateAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(default(TId), id);
            return id;
        }

        private async Task<T> ReadItemAsync<T>(TId id)
        {
            var result = await CrudStorage.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var readItem = (T)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(readItem);
            return readItem;
        }

        private async Task<T> UpdateItemAsync<T>(TId id, TypeOfTestDataEnum type)
            where T : IItemForTesting, new()
        {
            var updatedItem = new T();
            updatedItem.InitializeWithDataForTesting(type);
            await CrudStorage.UpdateAsync(id, updatedItem);
            return updatedItem;
        }
    }
}

