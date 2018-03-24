using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Logic;

namespace Xlent.Lever.Libraries2.Core.Cache
{
    public abstract class TestAutoCacheBase
    {
        protected IDistributedCache Cache;

        protected MemoryPersistance<string, Guid> Storage;
        protected DistributedCacheEntryOptions DistributedCacheOptions;
        protected static readonly Guid BaseGuid = Guid.NewGuid();
        protected static readonly string BaseGuidString = BaseGuid.ToString();
        protected AutoCacheOptions AutoCacheOptions;

        public virtual AutoCacheRead<string, Guid> AutoCacheRead { get; }

        protected async Task PrepareStorageAndCacheAsync(Guid id, string storageValue, string cacheValue)
        {
            await PrepareStorageAsync(id, storageValue);
            await PrepareCacheAsync(id, cacheValue);
        }

        protected async Task PrepareCacheAsync(Guid id, string cacheValue)
        {
            if (cacheValue == null)
            {
                await Cache.RemoveAsync(id.ToString());
            }
            else
            {
                await Cache.SetAsync(id.ToString(), AutoCacheRead.ToSerializedCacheEnvelope(cacheValue), DistributedCacheOptions);
            }
        }

        protected async Task PrepareStorageAsync(Guid id, string storageValue)
        {
            if (storageValue == null)
            {
                await Storage.DeleteAsync(id);
            }
            else
            {
                try
                {
                    var value = await Storage.ReadAsync(id);
                    if (value != storageValue) await Storage.UpdateAsync(id, storageValue);
                }
                catch (FulcrumNotFoundException)
                {
                    await Storage.CreateWithSpecifiedIdAsync(id, storageValue);
                }
            }
        }

        protected async Task VerifyAsync(Guid id, string expectedStorageValue, string expectedCacheValueBefore, string expectedReadValue, string expectedCacheValueAfter)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedCacheValueBefore, true);
            await VerifyRead(id, expectedReadValue);
            await VerifyCache(id, expectedCacheValueAfter, false);
        }

        protected async Task VerifyAsync(Guid id, string expectedStorageValue, string expectedCacheValueBefore, string expectedReadValue)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedCacheValueBefore, true);
            await VerifyRead(id, expectedReadValue);
            await VerifyCache(id, expectedReadValue, false);
        }

        protected async Task VerifyAsync(Guid id, string expectedStorageValue, string expectedCacheValueBefore)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedCacheValueBefore, true);
            await VerifyRead(id, expectedCacheValueBefore);
            await VerifyCache(id, expectedCacheValueBefore, false);
        }

        protected async Task VerifyAsync(Guid id, string expectedStorageValue)
        {
            await VerifyStorage(id, expectedStorageValue);
            await VerifyCache(id, expectedStorageValue, true);
            await VerifyRead(id, expectedStorageValue);
            await VerifyCache(id, expectedStorageValue, false);
        }

        protected async Task VerifyStorage(Guid id, string expectedStorageValue)
        {
            try
            {
                var actualStorageValue = await Storage.ReadAsync(id);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedStorageValue, actualStorageValue, "Storage verification failed.");
            }
            catch (FulcrumNotFoundException)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(expectedStorageValue, $"Nothing found in storage, but expected \"{expectedStorageValue}\".");
            }
        }

        protected async Task VerifyCache(Guid id, string expectedCacheValue, bool isBeforeRead)
        {
            var beforeOrAfter = isBeforeRead ? "before" : "after";
            var actualCacheSerializedValue = await Cache.GetAsync(id.ToString());
            if (actualCacheSerializedValue == null)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(expectedCacheValue, $"Cache value was null, but expected \"{expectedCacheValue}\" {beforeOrAfter} read.");
            }
            else
            {
                var actualCacheValue = SupportMethods.ToItem<string>(actualCacheSerializedValue);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedCacheValue, actualCacheValue, $"Cache verification {beforeOrAfter} read failed.");
            }
        }

        protected async Task VerifyRead(Guid id, string expectedReadValue)
        {
            try
            {
                var actualReadValue = await AutoCacheRead.ReadAsync(id);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedReadValue, actualReadValue, "AutoCacheRead Read verification failed.");
            }
            catch (FulcrumNotFoundException)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(expectedReadValue, $"Nothing found at read, but expected \"{expectedReadValue}\".");
            }
        }

        protected static Guid FromStringToGuid(string item)
        {
            return FromStringToGuid(item, int.MaxValue);
        }

        protected static Guid FromStringToGuid(string item, int maxLength)
        {
            if (item.Length > maxLength)
            {
                item = item.Substring(0, maxLength);
            }
            var itemAsInt = item.GetHashCode();
            var itemAsHex = itemAsInt.ToString("X");
            var itemLength = itemAsHex.Length;
            var totalLength = BaseGuidString.Length;
            var itemAsGuidString = BaseGuidString.Substring(0, totalLength - itemLength) + itemAsHex;
            return new Guid(itemAsGuidString);
        }
    }
}
