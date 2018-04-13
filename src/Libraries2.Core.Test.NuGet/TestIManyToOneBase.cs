using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;
using Xlent.Lever.Libraries2.MoveTo.Core.Crud.Helpers;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet
{
    public abstract class TestIManyToOneBase<TId, TReferenceId>
    {
        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract IManyToOneRelationComplete<TestItemManyToOne<TId, TReferenceId>, TId>
            ManyStorageRecursive { get; }

        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract
            IManyToOneRelationComplete<TestItemManyToOne<TId, TReferenceId>, TId>
            ManyStorageNonRecursive { get; }

        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICrd<TestItemId<TId>, TId> OneStorage { get; }

        protected async Task<TestItemManyToOne<TId, TReferenceId>> CreateItemAsync(
            ICrd<TestItemManyToOne<TId, TReferenceId>, TId> storage, TypeOfTestDataEnum type, TId parentId)
        {
            return await CreateItemAsync(storage, type,
                StorageHelper.ConvertToParameterType<TReferenceId>(parentId));
        }

        protected async Task<TestItemManyToOne<TId, TReferenceId>> CreateItemAsync(
                ICrd<TestItemManyToOne<TId, TReferenceId>, TId> storage, TypeOfTestDataEnum type, TReferenceId parentId)
            {
                var item = new TestItemManyToOne<TId, TReferenceId>();
            item.InitializeWithDataForTesting(type);
            item.ParentId = parentId;
            var createdItem = await storage.CreateAndReturnAsync(item);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(default(TId), createdItem);
            return createdItem;
        }

        protected async Task<TestItemId<TId>> CreateItemAsync(TypeOfTestDataEnum type)
        {
            var item = new TestItemId<TId>();
            item.InitializeWithDataForTesting(type);
            var createdItem = await OneStorage.CreateAndReturnAsync(item);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(default(TId), createdItem);
            return createdItem;
        }
    }
}