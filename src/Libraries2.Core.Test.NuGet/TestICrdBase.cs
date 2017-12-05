using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet
{
    public abstract class TestICrdBase<TItem, TId> 
        where TItem : IItemForTesting, new()
    {
        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICrd<TItem, TId> CrdStorage { get; }

        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICrud<TItem, TId> CrudStorage { get; }

        protected async Task<TId> CreateItemAsync(TypeOfTestDataEnum type)
        {
            var initialItem = new TItem();
            initialItem.InitializeWithDataForTesting(type);
            var id = await CrdStorage.CreateAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(default(TId), id);
            return id;
        }

        protected async Task<TItem> ReadItemAsync(TId id)
        {
            var result = await CrdStorage.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            var readItem = (TItem)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(readItem);
            return readItem;
        }

        protected async Task<TItem> UpdateItemAsync(TId id, TypeOfTestDataEnum type)
        {
            var updatedItem = new TItem();
            updatedItem.InitializeWithDataForTesting(type);
            if (updatedItem is IOptimisticConcurrencyControlByETag etaggedItem)
            {
                var readItem = await ReadItemAsync(id);
                etaggedItem.Etag = ((IOptimisticConcurrencyControlByETag)readItem).Etag;
            }
            await CrudStorage.UpdateAsync(id, updatedItem);
            return updatedItem;
        }
    }
}