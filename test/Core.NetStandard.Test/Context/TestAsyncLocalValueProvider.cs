using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.Context
{
    [TestClass]
    public class TestAsyncLocalValueProvider
    {
        [UT.TestMethod]
        public void TwoThreads1()
        {
            AsyncMethodA().Wait();
        }

        [UT.TestMethod]
        public void TwoThreads2()
        {
            const string stringA = "String A";
            const string stringB = "String B";
            var provider = new AsyncLocalValueProvider();
            new Thread(async () => await AsyncMethodB(provider, stringA)).Start();
            new Thread(async () => await AsyncMethodB(provider, stringB)).Start();
        }

        [UT.TestMethod]
        public void TwoProviders()
        {
            const string stringA = "String A";
            const string stringB = "String B";
            var provider1 = new AsyncLocalValueProvider();
            provider1.SetValue("X", stringA);
            var provider2 = new AsyncLocalValueProvider();
            UT.Assert.AreEqual(stringA, provider2.GetValue<string>("X"));
            provider2.SetValue("X", stringB);
            UT.Assert.AreEqual(stringB, provider1.GetValue<string>("X"));
        }

        [UT.TestMethod]
        public void GetNotInitialized()
        {
            var provider = new AsyncLocalValueProvider();
            UT.Assert.IsNull(provider.GetValue<string>("X"));
        }

        private static async Task AsyncMethodA()
        {
            const string stringA = "String A";
            const string stringB = "String B";
            var provider = new AsyncLocalValueProvider();
            var t1 = AsyncMethodB(provider, stringA);
            var t2 = AsyncMethodB(provider, stringB);
            await t1;
            await t2;
        }

        private static async Task AsyncMethodB(AsyncLocalValueProvider provider, string expectedValue)
        {
            provider.SetValue("X", expectedValue);
            UT.Assert.AreEqual(expectedValue, provider.GetValue<string>("X"));
            await Task.Delay(100);
            UT.Assert.AreEqual(expectedValue, provider.GetValue<string>("X"));
        }
    }
}
