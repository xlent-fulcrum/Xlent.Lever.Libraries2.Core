using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.Core.Error
{
    [TestClass]
    public class ErrorTest
    {
        [TestMethod]
        public void TestStackTrace()
        {
            var x = new FulcrumUnauthorizedException("x");
            // Just access StackTrace gave NRE before
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(x.StackTrace);
        }
    }
}
