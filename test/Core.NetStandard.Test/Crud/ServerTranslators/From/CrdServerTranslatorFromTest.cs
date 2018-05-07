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
        private Mock<ITranslatorService> _translatorServiceMock;
        private Mock<ICrd<TestModelCreate, TestModel, string>> _storageMock;
        private ICrd<TestModelCreate, TestModel, string> _serviceToTest;
        private const string ClientName = "client-name";
        private const string ServerName = "server-name";

        [TestInitialize]
        public void Initialize()
        {
            _translatorServiceMock = new Mock<ITranslatorService>();
            _storageMock = new Mock<ICrd<TestModel, string>>();
            var serverTranslator = new CrdServerTranslatorFrom<TestModelCreate, TestModel>(_storageMock.Object, TestModel.IdConceptName,
                () => ServerName);
            _serviceToTest = serverTranslator;
        }

        [TestMethod]
        public async Task CreateAsync()
        {
            _storageMock
                .Setup(crd =>
                    crd.CreateAsync(It.Is<TestModelCreate>(create => create.Status == "1"), CancellationToken.None))
                .ReturnsAsync("TM1");
            var item = new TestModelCreate()
            {
                Name = "name",
                Status = $"({TestModelCreate.StatusConceptName}!~{ClientName}!A)"
            };
            var id = await _serviceToTest.CreateAsync(item);
        }
    }
}
