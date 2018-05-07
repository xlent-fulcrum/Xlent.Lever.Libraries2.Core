using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.ServerTranslators.From;
using Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Crud.ServerTranslators.Support;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Crud.ServerTranslators.From
{
    [TestClass]
    public class CrdServerTranslatorFromTest
    {
        private Mock<ICrd<TestModelCreate, TestModel, string>> _storageMock;
        private ICrd<TestModelCreate, TestModel, string> _serviceToTest;
        private const string ClientName = "client-name";
        private const string ServerName = "server-name";
        private const string TestModelServerId = "server-1";
        private const string TestModelClientId = "client-1";
        private const string TestModelClientStatus = "client-status-a";
        private const string TestModelServerStatus = "server-status-a";

        [TestInitialize]
        public void Initialize()
        {
            _storageMock = new Mock<ICrd<TestModelCreate, TestModel, string>>();
            var serverTranslator = new CrdServerTranslatorFrom<TestModelCreate, TestModel>(_storageMock.Object, TestModel.IdConceptName,
                () => ServerName);
            _serviceToTest = serverTranslator;
        }

        [TestMethod]
        public async Task CreateAsync()
        {
            _storageMock
                .Setup(crd =>
                    crd.CreateAsync(It.Is<TestModelCreate>(create => create.Status == TestModelServerStatus), CancellationToken.None))
                .ReturnsAsync(TestModelServerId);
            var item = GetServerTestModel();
            var id = await _serviceToTest.CreateAsync(item);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(TestModel.DecoratedId(ServerName, TestModelServerId), id);
            _storageMock.Verify();
        }

        private static TestModel GetClientTestModel()
        {
            var item = new TestModel()
            {
                Id = TestModelClientId,
                Status = TestModelClientStatus
            };
            return item;
        }

        private static TestModel GetServerTestModel()
        {
            var item = new TestModel()
            {
                Id = TestModelServerId,
                Status = TestModelServerStatus
            };
            return item;
        }
    }
}
