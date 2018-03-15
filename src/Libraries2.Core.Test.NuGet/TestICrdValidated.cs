using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet
{
    /// <summary>
    /// Tests for testing any storage that implements <see cref="ICrud{TStorable,TId}"/>
    /// </summary>
    [TestClass]
    public abstract class TestICrdValidated<TId> : TestICrdBase<TestItemValidated<TId>, TId>
    {
        /// <summary>
        /// Try to create an item that is not valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FulcrumContractException))]
        public async Task Create_ValidationFailed_Async()
        {
            var initialItem = new TestItemValidated<TId>();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.ValidationFail);
            await CrdStorage.CreateAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail($"Expected the method {nameof(CrdStorage.CreateAsync)} to detect that the data was not valid and throw the exception {nameof(FulcrumContractException)}.");
        }

        /// <summary>
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task Create_Read_Validated_Async()
        {
            var initialItem = new TestItemValidated<TId>();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(default(TId), initialItem.Id);
            initialItem.Validate(null);
            var id = await CrdStorage.CreateAsync(initialItem);
            var createdItem = await CrdStorage.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(createdItem.Id, default(TId));
            createdItem.Validate(null);
            initialItem.Id = createdItem.Id;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }

        /// <summary>
        /// Try to create an item that is not valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FulcrumContractException))]
        public async Task CreateAndReturn_ValidationFailed_Async()
        {
            var initialItem = new TestItemValidated<TId>();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.ValidationFail);
            await CrdStorage.CreateAndReturnAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail($"Expected the method {nameof(CrdStorage.CreateAndReturnAsync)} to detect that the data was not valid and throw the exception {nameof(FulcrumContractException)}.");
        }

        /// <summary>
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task CreateAndReturn_Validated_Async()
        {
            var initialItem = new TestItemValidated<TId>();
            initialItem.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(default(TId), initialItem.Id);
            initialItem.Validate(null);
            var createdItem = await CrdStorage.CreateAndReturnAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(createdItem.Id, default(TId));
            createdItem.Validate(null);
            initialItem.Id = createdItem.Id;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(initialItem, createdItem);
        }
    }
}

