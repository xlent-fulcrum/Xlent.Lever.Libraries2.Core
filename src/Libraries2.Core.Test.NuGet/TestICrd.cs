using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet
{
    /// <summary>
    /// Tests for testing any storage that implements <see cref="ICrud{TStorable,TId}"/>
    /// </summary>
    [TestClass]
    public abstract class TestICrd<TStorableItem, TId>
        where TStorableItem : IItemForTesting<TStorableItem>, IStorableItem<TId>, IValidatable, new() 
    {
        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICrd<TStorableItem, TId> CrdStorage { get; }

        /// <summary>
        /// Create an item
        /// </summary>
        [TestMethod]
        public async Task Create()
        {
            var initialItem = new TStorableItem().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            Core.Assert.FulcrumAssert.IsValidated(initialItem);
            var createdItem = await CrdStorage.CreateAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(createdItem);
            FulcrumAssert.IsValidated(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(initialItem.Id,createdItem.Id);
            initialItem.Id = createdItem.Id;
            // ReSharper disable once SuspiciousTypeConversion.Global
            ValidateEtagChangeMakesItemsEqual(initialItem, createdItem);
        }

        /// <summary>
        /// Read an item
        /// </summary>
        [TestMethod]
        public async Task Read()
        {
            var initialItem = new TStorableItem().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            FulcrumAssert.IsValidated(initialItem);
            var createdItem = await CrdStorage.CreateAsync(initialItem);
            var readItem = await CrdStorage.ReadAsync(createdItem.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(createdItem, readItem);
        }

        /// <summary>
        /// Delete an item
        /// </summary>
        [TestMethod]
        public async Task Delete()
        {
            var initialItem = new TStorableItem().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            var createdItem = await CrdStorage.CreateAsync(initialItem);
            await CrdStorage.ReadAsync(createdItem.Id);
            await CrdStorage.DeleteAsync(createdItem.Id);
            try
            {
                await CrdStorage.ReadAsync(createdItem.Id);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("Expected an exception");
            }
            catch (FulcrumNotFoundException)
            {
                // As expected
            }
        }

        #region Support methods

        /// <summary>
        /// Validate that the two items are not equal, set the ETag, verify that they are equal. 
        /// </summary>
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        protected static void ValidateEtagChangeMakesItemsEqual(IStorableItem<TId> before, IStorableItem<TId> after)
        {
            if (!(before is IOptimisticConcurrencyControlByETag beforeEtag) 
                || !(after is IOptimisticConcurrencyControlByETag afterEtag)) return;

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(beforeEtag, afterEtag);
            beforeEtag.ETag = afterEtag.ETag;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(beforeEtag, afterEtag);
        }

        /// <summary>
        /// Validate that the two items are not equal, set the ETag, verify that they are still not equal. 
        /// </summary>
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        protected static void ValidateEtagChangeMakesNotItemsEqual(IStorableItem<TId> before, IStorableItem<TId> after)
        {
            if (!(before is IOptimisticConcurrencyControlByETag beforeEtag) 
                || !(after is IOptimisticConcurrencyControlByETag afterEtag)) return;

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(beforeEtag, afterEtag);
            beforeEtag.ETag = beforeEtag.ETag;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(beforeEtag, afterEtag);
        }
        #endregion
    }
}

