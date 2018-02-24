using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Health.Logic;
using Xlent.Lever.Libraries2.Core.Health.Model;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;

namespace Xlent.Lever.Libraries2.Core.Health
{
    [TestClass]
    public class HealthTest
    {
        private static readonly ITenant Tenant = new Tenant("Super", "Mario");
        private IResourceHealth2 _goombaResource;

        [TestInitialize]
        public void Initialize()
        {
            _goombaResource = new GoombaResource();
        }

        [TestMethod]
        public async Task TestWithIResourceHealth2()
        {
            const string resource = "Goomba";
            var aggregator = new ResourceHealthAggregator2(Tenant, resource);
            await aggregator.AddResourceHealthAsync("DB", _goombaResource);
            var aggregatedHealthResponse = aggregator.GetAggregatedHealthResponse();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(resource, aggregatedHealthResponse.Name);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(aggregatedHealthResponse.Resources);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, aggregatedHealthResponse.Resources.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(GoombaResource.Name, aggregatedHealthResponse.Resources.First().Resource);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(HealthInfo.StatusEnum.Ok, aggregatedHealthResponse.Resources.First().Status);
        }

        [TestMethod]
        public async Task TestWithDelegate()
        {
            const string resource = "Koopa";
            var aggregator = new ResourceHealthAggregator2(Tenant, resource);
            await aggregator.AddResourceHealthAsync("DB", HealthDelegateMethod);
            var aggregatedHealthResponse = aggregator.GetAggregatedHealthResponse();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(resource, aggregatedHealthResponse.Name);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(aggregatedHealthResponse.Resources);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, aggregatedHealthResponse.Resources.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("KOOPA", aggregatedHealthResponse.Resources.First().Resource);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(HealthInfo.StatusEnum.Ok, aggregatedHealthResponse.Resources.First().Status);
        }

        private static Task<HealthInfo> HealthDelegateMethod(ITenant tenant)
        {
            return Task.FromResult(new HealthInfo
            {
                Resource = "KOOPA",
                Status = HealthInfo.StatusEnum.Ok,
                Message = "Troopa"
            });
        }
    }

    public class GoombaResource : IResourceHealth2
    {
        public const string Name = "GOOMBA";

        public Task<HealthInfo> GetResourceHealth2Async(ITenant tenant)
        {
            return Task.FromResult(new HealthInfo
            {
                Resource = Name,
                Status = HealthInfo.StatusEnum.Ok,
                Message = "Kuribo"
            });
        }
    }
}
