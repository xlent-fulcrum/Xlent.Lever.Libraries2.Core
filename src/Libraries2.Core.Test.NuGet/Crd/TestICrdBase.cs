using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Test.NuGet.Model;

namespace Xlent.Lever.Libraries2.Core.Test.NuGet.Crd
{
    public abstract class TestICrdBase<TModelCreate, TModel, TId>
        where TModelCreate : IItemForTesting, new()
    where TModel : TModelCreate
    {
        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICrd<TModelCreate, TModel, TId> CrdStorage { get; }

        /// <summary>
        /// The storage that should be tested
        /// </summary>
        protected abstract ICrud<TModelCreate, TModel, TId> CrudStorage { get; }

        protected async Task<TId> CreateItemAsync(TypeOfTestDataEnum type)
        {
            var initialItem = new TModelCreate();
            initialItem.InitializeWithDataForTesting(type);
            var id = await CrdStorage.CreateAsync(initialItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(default(TId), id);
            return id;
        }

        protected async Task<TModel> ReadItemAsync(TId id)
        {
            var readItem = await CrdStorage.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(readItem);
            return readItem;
        }

        protected async Task<TModel> UpdateItemAsync(TId id, TypeOfTestDataEnum type)
        {
            var updatedItem = await ReadItemAsync(id);
            updatedItem.InitializeWithDataForTesting(type);
            if (updatedItem is IUniquelyIdentifiable<TId> itemWithId)
            {
                itemWithId.Id = id;
            }
            await CrudStorage.UpdateAsync(id, updatedItem);
            return await ReadItemAsync(id);
        }
    }
}