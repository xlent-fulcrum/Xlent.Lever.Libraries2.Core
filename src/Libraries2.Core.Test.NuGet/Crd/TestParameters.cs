using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;
using A = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet
{
    /// <summary>
    /// Tests for testing any storage that implements <see cref="ICrd{TModelCreate,TModel,TId}"/>
    /// </summary>
    [TestClass]
    public abstract class TestParameters
    {
        private readonly ICrd<TestItemBare, Guid> _implementation;

        public TestParameters(ICrd<TestItemBare, Guid> implementation)
        {
            _implementation = implementation;
        }

        /// <summary>
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task CreateWithNullItem()
        {
            await ExpectContractExceptionAsync(async () =>
                    await _implementation.CreateAsync(null),
                "CreateAsync(null)");
            await ExpectContractExceptionAsync(async () =>
                    await _implementation.CreateAndReturnAsync(null),
                "CreateAndReturnAsync(null)");
            await ExpectContractExceptionAsync(async () =>
                    await _implementation.CreateWithSpecifiedIdAsync(Guid.NewGuid(), null),
                "CreateWithSpecifiedIdAsync(Guid.NewGuid(), null)");
            await ExpectContractExceptionAsync(async () =>
                    await _implementation.CreateWithSpecifiedIdAndReturnAsync(Guid.NewGuid(), null),
                "CreateWithSpecifiedIdAndReturnAsync(Guid.NewGuid(), null)");
        }

        /// <summary>
        /// Create a bare item
        /// </summary>
        [TestMethod]
        public async Task TestWithDefaultId()
        {
            var item = new TestItemBare();
            item.InitializeWithDataForTesting(TypeOfTestDataEnum.Default);
            await ExpectContractExceptionAsync(async () =>
                    await _implementation.CreateWithSpecifiedIdAsync(Guid.Empty, item),
                "CreateWithSpecifiedIdAsync(Guid.Empty, item)");
            await ExpectContractExceptionAsync(async () =>
                    await _implementation.CreateWithSpecifiedIdAndReturnAsync(Guid.NewGuid(), null),
                "CreateWithSpecifiedIdAndReturnAsync(Guid.Empty, item)");
        }


        public delegate Task MethodDelegate();

        private async Task ExpectContractExceptionAsync(MethodDelegate action, string description)
        {
            try
            {
                await action();
                A.Fail($"{description} expected to throw exception (FulcrumContractException).");
            }
            catch (FulcrumContractException)
            {
                // This is where we expect to end up
            }
            catch (Exception e)
            {
                A.Fail($"{description} throw exception {e.GetType().Name}, but was expected to throw FulcrumContractException.");
            }
        }
    }
}

