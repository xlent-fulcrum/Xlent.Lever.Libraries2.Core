using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.NexusLink;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.NexusLink
{
    [TestClass]
    public class NexusTests
    {
        [TestMethod]
        public void MembersAreSet()
        {
            UT.Assert.IsNotNull(Nexus.Logger);
            UT.Assert.IsNotNull(Nexus.Assert.Critical);
            UT.Assert.IsNotNull(Nexus.Assert.Warning);
            UT.Assert.IsNotNull(Nexus.Require.Internal);
            UT.Assert.IsNotNull(Nexus.Require.Public);
            UT.Assert.IsNotNull(Nexus.Require.Api);
            UT.Assert.IsNotNull(Nexus.Expect.Internal);
            UT.Assert.IsNotNull(Nexus.Expect.Public);
        }
    }
}
