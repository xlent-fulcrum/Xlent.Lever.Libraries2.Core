using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Crud.Cache;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    public abstract class TestAutoCacheBase<TModelCreate, TModel>
    where TModel : TModelCreate
    {
        protected IDistributedCache Cache;

        protected virtual ICrudWithSpecifiedId<TModelCreate, TModel, Guid> CrudStorage { get; }
        protected DistributedCacheEntryOptions DistributedCacheOptions;
        protected readonly string BaseGuidString;
        protected AutoCacheOptions AutoCacheOptions;

        public virtual ReadAutoCache<TModel, Guid> ReadAutoCache { get; }

        protected TestAutoCacheBase()
        {
            BaseGuidString = Guid.NewGuid().ToString();
        }

        protected async Task PrepareStorageAndCacheAsync(Guid id, TModel storageValue, TModel cacheValue)
        {
            await PrepareStorageAsync(id, storageValue);
            await PrepareCacheAsync(id, cacheValue);
        }

        protected async Task PrepareCacheAsync(Guid id, TModel cacheValue)
        {
            if (cacheValue == null)
            {
                await Cache.RemoveAsync(id.ToString());
            }
            else
            {
                await Cache.SetAsync(id.ToString(), ReadAutoCache.ToSerializedCacheEnvelope(cacheValue), DistributedCacheOptions);
            }
        }

        protected async Task PrepareStorageAsync(Guid id, TModel storageValue)
        {
            if (storageValue == null)
            {
                await CrudStorage.DeleteAsync(id);
            }
            else
            {
                try
                {
                    var value = await CrudStorage.ReadAsync(id);
                    if (!Equals(value,storageValue)) await CrudStorage.UpdateAsync(id, storageValue);
                }
                catch (FulcrumNotFoundException)
                {
                    await CrudStorage.CreateWithSpecifiedIdAsync(id, storageValue);
                }
            }
        }

        protected async Task VerifyAsync(Guid id, TModel expectedStorageValue, TModel expectedCacheValueBefore, TModel expectedReadValue, TModel expectedCacheValueAfter)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedCacheValueBefore, true);
            await VerifyRead(id, expectedReadValue);
            await VerifyCache(id, expectedCacheValueAfter, false);
        }

        protected async Task VerifyAsync(Guid id, TModel expectedStorageValue, TModel expectedCacheValueBefore, TModel expectedReadValue)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedCacheValueBefore, true);
            await VerifyRead(id, expectedReadValue);
            await VerifyCache(id, expectedReadValue, false);
        }

        protected async Task VerifyAsync(Guid id, TModel expectedStorageValue, TModel expectedCacheValueBefore)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedCacheValueBefore, true);
            await VerifyRead(id, expectedCacheValueBefore);
            await VerifyCache(id, expectedCacheValueBefore, false);
        }

        protected async Task VerifyAsync(Guid id, TModel expectedStorageValue)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedStorageValue, true);
            await VerifyRead(id, expectedStorageValue);
            await VerifyCache(id, expectedStorageValue, false);
        }

        protected async Task VerifyStorage(Guid id, TModel expectedStorageValue)
        {
            try
            {
                var actualStorageValue = await CrudStorage.ReadAsync(id);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedStorageValue, actualStorageValue, "Storage verification failed.");
            }
            catch (FulcrumNotFoundException)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(expectedStorageValue, $"Nothing found in storage, but expected \"{expectedStorageValue}\".");
            }
        }

        protected async Task VerifyCache(Guid id, TModel expectedCacheValue, bool isBeforeRead)
        {
            var beforeOrAfter = isBeforeRead ? "before" : "after";
            var actualCacheSerializedValue = await Cache.GetAsync(id.ToString());
            if (actualCacheSerializedValue == null)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(expectedCacheValue, $"Cache value was null, but expected \"{expectedCacheValue}\" {beforeOrAfter} read.");
            }
            else
            {
                var actualCacheValue = SerializingSupport.ToItem<TModel>(actualCacheSerializedValue);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedCacheValue, actualCacheValue, $"Cache verification {beforeOrAfter} read failed.");
            }
        }

        protected async Task VerifyRead(Guid id, TModel expectedReadValue)
        {
            try
            {
                var actualReadValue = await ReadAutoCache.ReadAsync(id);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedReadValue, actualReadValue, "ReadAutoCache Read verification failed.");
            }
            catch (FulcrumNotFoundException)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(expectedReadValue, $"Nothing found at read, but expected \"{expectedReadValue}\".");
            }
        }

        protected Guid ToGuid(TModel item)
        {
            return ToGuid(item, int.MaxValue);
        }

        protected Guid ToGuid(TModel item, int maxLength)
        {
            var itemAsString = item.ToString(); 
            if (itemAsString.Length > maxLength)
            {
                itemAsString = itemAsString.Substring(0, maxLength);
            }
            var itemAsInt = itemAsString.GetHashCode();
            var itemAsHex = itemAsInt.ToString("X");
            var itemLength = itemAsHex.Length;
            var totalLength = BaseGuidString.Length;
            var itemAsGuidString = BaseGuidString.Substring(0, totalLength - itemLength) + itemAsHex;
            return new Guid(itemAsGuidString);
        }
    }
}
