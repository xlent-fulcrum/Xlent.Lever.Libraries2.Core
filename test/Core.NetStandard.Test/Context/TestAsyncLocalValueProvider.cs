using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Context;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Context
{
    [TestClass]
    public class TestAsyncLocalValueProvider
    {
        private AsyncLocalValueProvider _provider;
        private readonly object _lockObject = new object();
        private string _latestSet;

        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(nameof(TestAsyncLocalValueProvider));
            _provider = new AsyncLocalValueProvider();
        }
        [TestMethod]
        public void GetNotInitialized()
        {
            UT.Assert.IsNull(_provider.GetValue<string>("X"));
        }

        [TestMethod]
        public async Task TaskCanReadTop()
        {
            const string topString = "LocalString";
            SetValue(_provider, topString);
            var task = GetValueAsync(_provider);
            var value = await task;
            UT.Assert.AreEqual(topString, value);
        }

        [TestMethod]
        public async Task TaskCanUpdateTop()
        {
            const string topString = "LocalString1";
            const string taskString = "TaskString";
            SetValue(_provider, topString);
            await SetValueAsync(_provider, taskString);
            UT.Assert.AreEqual(taskString, GetValue(_provider));
        }

        [TestMethod]
        public async Task TaskCanUpdateTopConfigureAwait()
        {
            const string topString = "LocalString";
            const string taskString = "TaskString";
            SetValue(_provider, topString);
            await SetValueAsync(_provider, taskString).ConfigureAwait(false);
            UT.Assert.AreEqual(taskString, GetValue(_provider));
        }

        [TestMethod]
        public async Task UNEXPECTED_ValueCreatedByTaskDoesNotReachTop()
        {
            const string taskString = "TaskString";
            // Same as above except we comment out the row below
            // SetValue(provider, topString);
            await SetValueAsync(_provider, taskString);
            UT.Assert.AreEqual(taskString, _latestSet);
            UT.Assert.IsNull(GetValue(_provider));
        }

        [TestMethod]
        public async Task TopUpdateAffectsTask()
        {
            const string localString1 = "LocalString1";
            const string localString2 = "LocalString2";
            const string taskString = "TaskString";
            SetValue(_provider, localString1);
            var task = CanSurviveContextChange(_provider, taskString);
            await Task.Delay(10);
            UT.Assert.AreEqual(taskString, _latestSet);
            SetValue(_provider, localString2);
            var confirmed = await task;
            UT.Assert.AreEqual(localString2, _latestSet);
            UT.Assert.IsFalse(confirmed);
            UT.Assert.AreEqual(localString2, GetValue(_provider));
        }

        [TestMethod]
        public async Task TwoAsyncTaskUpdateCollides()
        {
            const string topString = "LocalString";
            const string string1 = "String 1";
            const string string2 = "String 2";
            SetValue(_provider, topString);
            var separateContexts = await SetValueInTwoTasksAsync(string1, string2);
            UT.Assert.AreEqual(string2, GetValue(_provider));
            UT.Assert.IsFalse(separateContexts);
        }

        /// <summary>
        /// An async task is affected by a change in the caller
        /// </summary>
        [TestMethod]
        public async Task UNEXPECTED_TwoAsyncTaskCreateDoesNotCollide()
        {
            const string string1 = "String 1";
            const string string2 = "String 2";
            var separateContexts = await SetValueInTwoTasksAsync(string1, string2);
            UT.Assert.IsNull(GetValue(_provider));
            UT.Assert.IsTrue(separateContexts);
        }

        /// <summary>
        /// The threads does not affect each other because they are on different contexts.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TwoConcurrentThreadsHasDifferentCreateContexts()
        {
            const string stringA = "String A";
            const string stringB = "String B";
            var t1 = Task.Run(async () => UT.Assert.IsTrue(await CanSurviveContextChange(_provider, stringA)));
            await Task.Delay(10);
            UT.Assert.AreEqual(stringA, _latestSet);
            var t2 = Task.Run(() => SetValue(_provider, stringB));
            await Task.Delay(10);
            UT.Assert.AreEqual(stringB, _latestSet);
            await Task.WhenAll(t1, t2);
        }

        /// <summary>
        /// The threads does not affect each other because they are on different contexts.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UNEXPECTED_TwoConcurrentThreadsHasSameUpdateContexts()
        {
            const string stringA = "String A";
            const string stringB = "String B";
            SetValue(_provider, "InitialValue");
            var t1 = Task.Run(async () => UT.Assert.IsFalse(await CanSurviveContextChange(_provider, stringA)));
            await Task.Delay(10);
            UT.Assert.AreEqual(stringA, _latestSet);
            var t2 = Task.Run(() => SetValue(_provider, stringB));
            await Task.Delay(10);
            UT.Assert.AreEqual(stringB, _latestSet);
            await Task.WhenAll(t1, t2);
        }

        /// <summary>
        /// Top level and sub thread have different contexts.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SeparateThreadCanAccessTop()
        {
            const string topString = "LocalString";
            SetValue(_provider, topString);
            await Task.Run(() => UT.Assert.AreEqual(topString,  GetValue(_provider)));
        }

        /// <summary>
        /// Top level and sub thread have different contexts.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UNEXPECTED_SeparateThreadCanUpdateTop()
        {
            const string topString = "TopString";
            const string threadString = "ThreadString";
            SetValue(_provider, topString);
            await Task.Run(() => SetValue(_provider, threadString));
            UT.Assert.AreEqual(threadString, _latestSet);
            UT.Assert.AreEqual(threadString, GetValue(_provider));
        }

        [TestMethod]
        public void TwoProvidersHaveTheSameContext()
        {
            const string stringA = "String A";
            const string stringB = "String B";
            SetValue(_provider, stringA);
            var provider2 = new AsyncLocalValueProvider();
            UT.Assert.AreEqual(stringA, GetValue(_provider));
            SetValue(provider2, stringB);
            UT.Assert.AreEqual(stringB, GetValue(provider2));
            UT.Assert.AreEqual(stringB, GetValue(_provider));
        }

        private async Task<bool> SetValueInTwoTasksAsync(string string1, string string2)
        {
            var task = CanSurviveContextChange(_provider, string1);
            await Task.Delay(10);
            UT.Assert.AreEqual(string1, _latestSet);
            await SetValueAsync(_provider, string2);
            UT.Assert.AreEqual(string2, _latestSet);
            var confirmed = await task;
            return confirmed;
        }

        private async Task<bool> CanSurviveContextChange(IValueProvider provider, string localValue)
        {
            SetValue(provider, localValue);
            var count = 0;
            while (count < 50 && _latestSet == localValue)
            {
                await Task.Delay(10);
                count++;
            }
            return _latestSet != localValue && localValue == GetValue(provider);
        }
        
        private async Task SetValueAsync(IValueProvider provider, string value)
        {
            SetValue(provider, value);
            await Task.Yield();
        }

        private void SetValue(IValueProvider provider, string value)
        {
            lock (_lockObject)
            {
                provider.SetValue("X", value);
                _latestSet = value;
            }
        }

        private static async Task<string> GetValueAsync(IValueProvider provider)
        {
            return await Task.FromResult(provider.GetValue<string>("X"));
        }

        private static string GetValue(IValueProvider provider)
        {
            return provider.GetValue<string>("X");
        }
    }
}
