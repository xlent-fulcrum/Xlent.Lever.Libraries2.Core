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
    public abstract class TestICrdValidated<TId> : TestICrdBase<TestItemValidated, TId>
    {
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
    }
}

