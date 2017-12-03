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
    public abstract class TestICrd<TId>
    {
        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICrd<IItemForTesting, TId> CrdStorage { get; }

        #region CreateAsync_ReadAsync

        /// <summary>
        /// Try to create an item that is not valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FulcrumContractException))]
        public async Task Create_ValidationFailed_Async()
        {
            var initialItem = new TestItemValidated();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.ValidationFail);
            await CrdStorage.CreateAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail($"Expected the method {nameof(CrdStorage.CreateAsync)} to detect that the data was not valid and throw the exception {nameof(FulcrumContractException)}.");
        }

        /// <summary>
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task Create_Read_Bare_Async()
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
        public async Task Create_Read_Validated_Async()
        {
            var initialItem = new TestItemValidated();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            initialItem.Validate(null);
            var id = await CrdStorage.CreateAsync(initialItem);
            var result = await CrdStorage.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var createdItem = result as TestItemValidated;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            createdItem.Validate(null);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }

        /// <summary>
        /// Create an item with an id.
        /// </summary>
        [TestMethod]
        public async Task Create_Read_Id_Async()
        {
            var initialItem = new TestItemId<TId>();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(default(TId), initialItem.Id);
            var id = await CrdStorage.CreateAsync(initialItem);
            var result = await CrdStorage.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var createdItem = result as TestItemId<TId>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(default(TId), createdItem.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(id, createdItem.Id);
            initialItem.Id = createdItem.Id;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }

        /// <summary>
        /// Create an item with an id.
        /// </summary>
        [TestMethod]
        public async Task Create_Read_Etag_Async()
        {
            var initialItem = new TestItemEtag();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(default(TId), initialItem.Etag);
            var id = await CrdStorage.CreateAsync(initialItem);
            var result = await CrdStorage.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var createdItem = result as TestItemEtag;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(default(TId), createdItem.Etag);
            initialItem.Etag = createdItem.Etag;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }

        #endregion

        #region CreateAndReturnAsync

        /// <summary>
        /// Try to create an item that is not valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FulcrumContractException))]
        public async Task CreateAndReturn_ValidationFailed_Async()
        {
            var initialItem = new TestItemValidated();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.ValidationFail);
            await CrdStorage.CreateAndReturnAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail($"Expected the method {nameof(CrdStorage.CreateAndReturnAsync)} to detect that the data was not valid and throw the exception {nameof(FulcrumContractException)}.");
        }

        /// <summary>
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task CreateAndReturn_Bare_Async()
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
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task CreateAndReturn_Validated_Async()
        {
            var initialItem = new TestItemValidated();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            initialItem.Validate(null);
            var result = await CrdStorage.CreateAndReturnAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var createdItem = result as TestItemValidated;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            createdItem.Validate(null);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }

        /// <summary>
        /// Create an item with an id.
        /// </summary>
        [TestMethod]
        public async Task CreateAndReturn_Id_Async()
        {
            var initialItem = new TestItemId<TId>();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(default(TId), initialItem.Id);
            var result = await CrdStorage.CreateAndReturnAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var createdItem = result as TestItemId<TId>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(default(TId), createdItem.Id);
            initialItem.Id = createdItem.Id;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }

        /// <summary>
        /// Create an item with an id.
        /// </summary>
        [TestMethod]
        public async Task CreateAndReturn_Read_Etag_Async()
        {
            var initialItem = new TestItemEtag();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(default(TId), initialItem.Etag);
            var result = await CrdStorage.CreateAndReturnAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var createdItem = result as TestItemEtag;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(default(TId), createdItem.Etag);
            initialItem.Etag = createdItem.Etag;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }

        #endregion

        #region ReadAsync
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
        #endregion

        #region DeleteAsync
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
        #endregion
    }
}

