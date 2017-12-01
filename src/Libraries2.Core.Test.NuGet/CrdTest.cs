using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet
{
    /// <summary>
    /// Tests for testing any storage that implements <see cref="ICrud{TStorable,Guid}"/>
    /// </summary>
    public abstract class CrdTest<TId>
    {
        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICrd<PersonStorableItem<TId>, TId> GetStorage();

        #region Sunshine cases

        [Test]
        /// <summary>
        /// Create an item
        /// </summary>
        public async Task Create()
        {
            var methodName = $"{typeof(CrdTest<>).FullName}.{typeof(CrdTest<>).GetTypeInfo().GetDeclaredMethod(nameof(Create)).Name}";
            var initialItem = new TTestableItem().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            FulcrumAssert.IsValidated(initialItem, $"{methodName}: D125B3C3-F820-4986-8D36-AC516EAF778F");
            var createdItem = await Storage.CreateAsync(initialItem);
            FulcrumAssert.IsNotNull(createdItem, $"{methodName}: 3932F511-D9A1-4ECB-8E1B-024F86F5C02F");
            FulcrumAssert.IsValidated(createdItem, $"{methodName}: 6E5C1D55-13D4-49AB-97FF-B9D822D718BE");
            FulcrumAssert.AreNotEqual(initialItem.Id,createdItem.Id, $"{methodName}: 00A21FB8-392F-44B3-9F81-4FB3F82E38CE");
            initialItem.Id = createdItem.Id;
            // ReSharper disable once SuspiciousTypeConversion.Global
            ValidateEtagChangeMakesItemsEqual(initialItem, createdItem, $"{methodName}: 377B90F9-1111-4178-ADFE-560D4D44397A");
        }

        /// <summary>
        /// Read an item
        /// </summary>

        public async Task Read()
        {
            var methodName = $"{typeof(CrdTest<>).FullName}.{typeof(CrdTest<>).GetTypeInfo().GetDeclaredMethod(nameof(Read)).Name}";
            var initialItem = new TTestableItem().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            FulcrumAssert.IsValidated(initialItem, $"{methodName}.{nameof(Read)}#1");
            var createdItem = await Storage.CreateAsync(initialItem);
            var readItem = await Storage.ReadAsync(createdItem.Id);
            FulcrumAssert.AreEqual(createdItem, readItem, $"{methodName}: C0383E48-D4CB-4DCD-A84B-719F95D130CB");
        }

        /// <summary>
        /// Update an item
        /// </summary>

        public async Task Update()
        {
            var methodName = $"{typeof(CrdTest<>).FullName}.{typeof(CrdTest<>).GetTypeInfo().GetDeclaredMethod(nameof(Update)).Name}";
            var initialItem = new TTestableItem().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            var createdItem = await Storage.CreateAsync(initialItem);
            createdItem.ChangeDataToNotEqualForTesting();
            var updatedItem = await Storage.UpdateAsync(createdItem);
            FulcrumAssert.AreNotEqual(createdItem, updatedItem, $"{methodName}: 99634A93-426C-4E3E-BA42-5040E5CA7755");
            ValidateEtagChangeMakesNotItemsEqual(createdItem, updatedItem, $"{methodName}: 02BD4A39-6BC3-4A86-9B8C-24723180B48A");
        }


        /// <summary>
        /// Update and then read an item
        /// </summary>
        public async Task UpdateRead()
        {
            var methodName = $"{typeof(CrdTest<>).FullName}.{typeof(CrdTest<>).GetTypeInfo().GetDeclaredMethod(nameof(UpdateRead)).Name}";
            var initialItem = new TTestableItem().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            var createdItem = await Storage.CreateAsync(initialItem);
            createdItem.ChangeDataToNotEqualForTesting();
            var updatedItem = await Storage.UpdateAsync(createdItem);
            var readItem = await Storage.ReadAsync(createdItem.Id);
            FulcrumAssert.AreEqual(updatedItem, readItem, $"{methodName}: 9D3FCD55-C1EA-451E-9A63-CF9543B25273");
        }

        /// <summary>
        /// Delete an item
        /// </summary>

        public async Task Delete()
        {
            var methodName = $"{typeof(CrdTest<>).FullName}.{typeof(CrdTest<>).GetTypeInfo().GetDeclaredMethod(nameof(Delete)).Name}";
            var initialItem = new TTestableItem().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            var createdItem = await Storage.CreateAsync(initialItem);
            await Storage.ReadAsync(createdItem.Id);
            await Storage.DeleteAsync(createdItem.Id);
            try
            {
                await Storage.ReadAsync(createdItem.Id);
                FulcrumAssert.Fail("Expected an exception", $"{methodName}: C5570E61-F03D-46FD-A397-CA11BE2A5D0F");
            }
            catch (FulcrumNotFoundException)
            {
                // As expected
            }
        }
        #endregion

        #region private

        private async Task PrepareTestCase()
        {
            await Storage.DeleteAllAsync();
        }

        /// <summary>
        /// Validate that the two items are not equal, set the ETag, verify that they are equal. 
        /// </summary>
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        private static void ValidateEtagChangeMakesItemsEqual(IStorableItem<TId> before, IStorableItem<TId> after, string errorLocation)
        {
            var beforeEtag = before as IOptimisticConcurrencyControlByETag;
            var afterEtag = after as IOptimisticConcurrencyControlByETag;
            if (beforeEtag == null || afterEtag == null) return;

            FulcrumAssert.AreNotEqual(beforeEtag, afterEtag, errorLocation);
            beforeEtag.ETag = afterEtag.ETag;
            FulcrumAssert.AreEqual(beforeEtag, afterEtag, errorLocation);
        }

        /// <summary>
        /// Validate that the two items are not equal, set the ETag, verify that they are still not equal. 
        /// </summary>
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        private static void ValidateEtagChangeMakesNotItemsEqual(IStorableItem<TId> before, IStorableItem<TId> after, string errorLocation)
        {
            var beforeEtag = before as IOptimisticConcurrencyControlByETag;
            var afterEtag = after as IOptimisticConcurrencyControlByETag;
            if (beforeEtag == null || afterEtag == null) return;

            FulcrumAssert.AreNotEqual(beforeEtag, afterEtag, errorLocation);
            beforeEtag.ETag = beforeEtag.ETag;
            FulcrumAssert.AreNotEqual(beforeEtag, afterEtag, errorLocation);
        }
        #endregion
    }
}

