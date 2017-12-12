using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet
{
    /// <summary>
    /// Tests for testing any storage that implements <see cref="ICrud{TStorable,TId}"/>
    /// </summary>
    [TestClass]
    public abstract class TestICrudEtag<TId> : TestICrdEtag<TId>
    {
        protected override ICrd<TestItemEtag, TId> CrdStorage => CrudStorage;

        /// <summary>
        /// Create an item with an id.
        /// </summary>
        [TestMethod]
        public async Task Update_Read_Etag_Async()
        {
            var id = await CreateItemAsync(TypeOfTestDataEnum.Variant1);
            var updateItem = await UpdateItemAsync(id, TypeOfTestDataEnum.Variant2);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(updateItem);
            var readItem = await ReadItemAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(readItem);
            if (!updateItem.Equals(readItem))
            {
                updateItem.Etag = readItem.Etag;
            }
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(updateItem, readItem);
        }
    }
}

