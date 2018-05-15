using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.ServerTranslators.From;
using Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Crud.ServerTranslators.Support;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Crud.ServerTranslators.From
{
    [TestClass]
    public class CrdFromServerTranslatorTest : ServerTranslatorBase
    {
        private ICrd<TestModelCreate, TestModel, string> _serviceToTest;

        [TestInitialize]
        public void Initialize()
        {
            StorageMock = new Mock<ICrd<TestModelCreate, TestModel, string>>();
            var serverTranslator = new CrdFromServerTranslator<TestModelCreate, TestModel>(StorageMock.Object, TestModel.IdConceptName,
                () => ServerName);
            _serviceToTest = serverTranslator;
        }

        [TestMethod]
        public async Task CreateAsync_Sunshine()
        {
            StorageMock
                .Setup(crd =>
                    crd.CreateAsync(It.Is<TestModelCreate>(create => create.Status == TestModelServerStatus), CancellationToken.None))
                .ReturnsAsync(TestModelServerId);
            var serverInItem = GetServerTestModel();
            var id = await _serviceToTest.CreateAsync(serverInItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(TestModel.DecoratedId(ServerName, TestModelServerId), id);
            StorageMock.Verify();
        }

        [TestMethod]
        public async Task CreateAndReturnAsync_Sunshine()
        {
            var expectedItem = Decorate(ServerName, GetServerTestModel());
            var serverInItem = GetServerTestModel();
            var serverOutItem = GetServerTestModel();
            StorageMock
                .Setup(crd =>
                    crd.CreateAndReturnAsync(It.Is<TestModelCreate>(create => create.Status == TestModelServerStatus), CancellationToken.None))
                .ReturnsAsync(serverOutItem);
            var decoratedItem = await _serviceToTest.CreateAndReturnAsync(serverInItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(decoratedItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedItem, decoratedItem);
            StorageMock.Verify();
        }

        [TestMethod]
        public async Task CreateWithSpecifiedIdAsync_Sunshine()
        {
            StorageMock
                .Setup(crd =>
                    crd.CreateWithSpecifiedIdAsync(It.Is<string>(id => id == TestModelServerId),
                        It.Is<TestModelCreate>(create => create.Status == TestModelServerStatus),
                        CancellationToken.None)).Returns(Task.CompletedTask);
            var serverInItem = GetServerTestModel();
            await _serviceToTest.CreateWithSpecifiedIdAsync(TestModelServerId, serverInItem);
            StorageMock.Verify();
        }

        [TestMethod]
        public async Task CreateWithSpecifiedIdAndReturnAsync_Sunshine()
        {
            var expectedItem = Decorate(ServerName, GetServerTestModel());
            var serverInItem = GetServerTestModel();
            var serverOutItem = GetServerTestModel();
            StorageMock
                .Setup(crd =>
                    crd.CreateWithSpecifiedIdAndReturnAsync(It.Is<string>(id => id == TestModelServerId), It.Is<TestModelCreate>(create => create.Status == TestModelServerStatus), CancellationToken.None))
                .ReturnsAsync(serverOutItem);
            var decoratedItem = await _serviceToTest.CreateWithSpecifiedIdAndReturnAsync(TestModelServerId, serverInItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(decoratedItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedItem, decoratedItem);
            StorageMock.Verify();
        }

        [TestMethod]
        public async Task ReadAsync_Sunshine()
        {
            var expectedItem = Decorate(ServerName, GetServerTestModel());
            var serverOutItem = GetServerTestModel();
            StorageMock
                .Setup(crd =>
                    crd.ReadAsync(It.Is<string>(id => id == TestModelServerId),
                        CancellationToken.None)).ReturnsAsync(serverOutItem);
            var decoratedItem = await _serviceToTest.ReadAsync(TestModelServerId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(decoratedItem);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedItem, decoratedItem);
            StorageMock.Verify();
        }

        [TestMethod]
        public async Task ReadAllAsync_Sunshine()
        {
            var expectedItems = new[] { Decorate(ServerName, GetServerTestModel()) };

            var serverOutItems = new[] { GetServerTestModel() };
            StorageMock
                .Setup(crd =>
                    crd.ReadAllAsync(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync(serverOutItems);
            var decoratedItems = await _serviceToTest.ReadAllAsync();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(decoratedItems);
            AreEqual(expectedItems, decoratedItems);
            StorageMock.Verify();
        }

        [TestMethod]
        public async Task ReadAllWithPagingAsync_Sunshine()
        {
            var expectedItems = new[] { Decorate(ServerName, GetServerTestModel()) };
            var expectedPage = new PageEnvelope<TestModel>(0, 50, 1, expectedItems);

            var serverOutItems = new[] { GetServerTestModel() };
            var pageOut = new PageEnvelope<TestModel>(expectedPage.PageInfo, serverOutItems);
            StorageMock
                .Setup(crd =>
                    crd.ReadAllWithPagingAsync(0, It.IsAny<int?>(), CancellationToken.None)).ReturnsAsync(pageOut);
            var decoratedPage = await _serviceToTest.ReadAllWithPagingAsync(0);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(decoratedPage?.Data);
            AreEqual(expectedPage, decoratedPage);
            StorageMock.Verify();
        }

        [TestMethod]
        public async Task DeleteAsync_Sunshine()
        {
            StorageMock
                .Setup(crd =>
                    crd.DeleteAsync(It.Is<string>(id => id == TestModelServerId),
                        CancellationToken.None)).Returns(Task.CompletedTask);
            await _serviceToTest.DeleteAsync(TestModelServerId);
            StorageMock.Verify();
        }

        [TestMethod]
        public async Task DeleteAllAsync_Sunshine()
        {
            StorageMock
                .Setup(crd =>
                    crd.DeleteAllAsync(CancellationToken.None)).Returns(Task.CompletedTask);
            await _serviceToTest.DeleteAllAsync();
            StorageMock.Verify();
        }
    }
}
