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
    public abstract class TestICrud<TId> : TestICrd<TId>
    {
        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICrud<PersonStorableItem<TId>, TId> CrudStorage { get; }

        protected override ICrd<PersonStorableItem<TId>, TId> CrdStorage => CrudStorage;

        /// <summary>
        /// Update an item
        /// </summary>
        [TestMethod]
        public async Task Update()
        {
            var initialItem = new PersonStorableItem<TId>().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            var createdItem = await CrudStorage.CreateAsync(initialItem);
            createdItem.ChangeDataToNotEqualForTesting();
            var updatedItem = await CrudStorage.UpdateAsync(createdItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(createdItem, updatedItem);
            ValidateEtagChangeMakesNotItemsEqual(createdItem, updatedItem);
        }

        /// <summary>
        /// Update and then read an item
        /// </summary>
        [TestMethod]
        public async Task UpdateRead()
        {
            var initialItem = new PersonStorableItem<TId>().InitializeWithDataForTesting(TypeOfTestDataEnum.Variant1);
            var createdItem = await CrudStorage.CreateAsync(initialItem);
            createdItem.ChangeDataToNotEqualForTesting();
            var updatedItem = await CrudStorage.UpdateAsync(createdItem);
            var readItem = await CrudStorage.ReadAsync(createdItem.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(updatedItem, readItem);
        }
    }
}

