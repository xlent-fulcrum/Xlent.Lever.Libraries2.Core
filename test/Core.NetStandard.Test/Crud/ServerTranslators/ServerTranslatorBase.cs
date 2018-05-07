using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.ServerTranslators.From;
using Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Crud.ServerTranslators.Support;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Crud.ServerTranslators.From
{
    [TestClass]
    public class ServerTranslatorBase
    {
        protected Mock<ICrd<TestModelCreate, TestModel, string>> StorageMock;
        protected const string ClientName = "client-name";
        protected const string ServerName = "server-name";
        protected const string TestModelServerId = "server-1";
        protected const string TestModelClientId = "client-1";
        protected const string TestModelClientStatus = "client-status-a";
        protected const string TestModelServerStatus = "server-status-a";

        protected static TestModel Decorate(string clientName, TestModel source)
        {
            source.Id = source.DecoratedId(clientName);
            Decorate(clientName, (TestModelCreate) source);
            return source;
        }

        protected static TestModelCreate Decorate(string clientName, TestModelCreate source)
        {
            source.Status = source.DecoratedStatus(clientName);
            return source;
        }

        protected static TestModel GetClientTestModel()
        {
            var item = new TestModel()
            {
                Id = TestModelClientId,
                Status = TestModelClientStatus
            };
            return item;
        }

        protected static TestModel GetServerTestModel()
        {
            var item = new TestModel()
            {
                Id = TestModelServerId,
                Status = TestModelServerStatus
            };
            return item;
        }

        protected static void AreEqual<T>(IEnumerable<T> expectedList, IEnumerable<T> actualList)
        {
            AreEqual(expectedList.ToArray(), actualList.ToArray());
        }

        protected static void AreEqual<T>(IReadOnlyList<T> expectedArray, IReadOnlyList<T> actualArray)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedArray.Count, actualArray.Count);
            for (var i = 0; i < expectedArray.Count; i++)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedArray[i], actualArray[i]);
            }
        }

        protected static void AreEqual<T>(PageEnvelope<T> expectedPage, PageEnvelope<T> actualPage)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedPage.PageInfo, actualPage.PageInfo);
            AreEqual(expectedPage.Data.ToArray(), actualPage.Data.ToArray());
        }
    }
}
