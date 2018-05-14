using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Crud.Cache;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Cache
{
    public abstract class TestAutoCacheBase<TModelCreate, TModel>
    where TModel : TModelCreate
    {
        protected IDistributedCache Cache;

        protected virtual ICrud<TModelCreate, TModel, Guid> CrudStorage { get; }
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
                var value = await CrudStorage.ReadAsync(id);
                if (value == null) await CrudStorage.CreateWithSpecifiedIdAsync(id, storageValue);
                else if (!Equals(value, storageValue)) await CrudStorage.UpdateAsync(id, storageValue);
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
            var actualStorageValue = await CrudStorage.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedStorageValue, actualStorageValue, "Storage verification failed.");
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
            var actualReadValue = await ReadAutoCache.ReadAsync(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedReadValue, actualReadValue, "ReadAutoCache Read verification failed.");
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
