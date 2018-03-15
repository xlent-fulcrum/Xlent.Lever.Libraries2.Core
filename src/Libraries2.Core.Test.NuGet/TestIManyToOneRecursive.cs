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
    public abstract class TestIManyToOneRecursive<TId, TReferenceId> : TestIManyToOneBase<TId, TReferenceId>
    { 
        /// <summary>
        /// Create a recursive relation
        /// </summary>
        [TestMethod]
        public async Task SimpleRelationAsync()
        {
            var parent = await CreateItemAsync(ManyStorageRecursive, TypeOfTestDataEnum.Variant1, default(TReferenceId));
            var child = await CreateItemAsync(ManyStorageRecursive, TypeOfTestDataEnum.Variant2, parent.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(child.ParentId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(default(TReferenceId), child.ParentId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(parent.Value, child.Value);
            var foundParent = await ManyStorageRecursive.ReadParentAsync(child.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(foundParent);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(parent.Value, foundParent.Value);
        }
    }
}

