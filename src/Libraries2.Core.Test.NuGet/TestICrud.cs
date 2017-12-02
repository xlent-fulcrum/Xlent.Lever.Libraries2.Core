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
    public abstract class TestICrud<TStorableItem, TId> : TestICrd<TStorableItem, TId>
        where TStorableItem : IItemForTesting<TStorableItem>, IStorableItem<TId>, IValidatable, new()
    {
        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICrud<TStorableItem, TId> CrudStorage { get; }

        protected override ICrd<TStorableItem, TId> CrdStorage => CrudStorage;

        /// <summary>
        /// Update an item
        /// </summary>
        [TestMethod]
        public async Task Update()
        {
            var initialItem = new TStorableItem().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            var createdItem = await CrudStorage.CreateAndReturnAsync(initialItem);
            createdItem.ChangeDataToNotEqualForTesting();
            var updatedItem = await CrudStorage.UpdateAndReturnAsync(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(createdItem, updatedItem);
            ValidateEtagChangeMakesNotItemsEqual(createdItem, updatedItem);
        }

        /// <summary>
        /// Update and then read an item
        /// </summary>
        [TestMethod]
        public async Task UpdateRead()
        {
            var initialItem = new TStorableItem().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            var createdItem = await CrudStorage.CreateAndReturnAsync(initialItem);
            createdItem.ChangeDataToNotEqualForTesting();
            var updatedItem = await CrudStorage.UpdateAndReturnAsync(createdItem);
            var readItem = await CrudStorage.ReadAsync(createdItem.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(updatedItem, readItem);
        }
    }
}

