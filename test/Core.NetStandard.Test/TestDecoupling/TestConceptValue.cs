using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Decoupling.Model;

namespace Xlent.Lever.Libraries2.Core.TestDecoupling
{
    [TestClass]
    public class TestConceptValue
    {
        [TestMethod]
        public void ParseContext()
        {
            var conceptValue = ConceptValue.Parse("(concept!context!value)");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("concept", conceptValue.ConceptName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("context", conceptValue.ContextName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("value", conceptValue.Value);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(conceptValue.ClientName);
        }

        [TestMethod]
        public void ParseClient()
        {
            var conceptValue = ConceptValue.Parse("(concept!~client!value)");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("concept", conceptValue.ConceptName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("client", conceptValue.ClientName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("value", conceptValue.Value);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(conceptValue.ContextName);
        }

        [TestMethod]
        public void ContextToPath()
        {
            var conceptValue = new ConceptValue
            {
                ConceptName = "concept",
                ContextName = "context",
                Value = "value"
            };
            var path = conceptValue.ToPath();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("(concept!context!value)", path);
        }

        [TestMethod]
        public void ClientToPath()
        {
            var conceptValue = new ConceptValue
            {
                ConceptName = "concept",
                ClientName = "client",
                Value = "value"
            };
            var path = conceptValue.ToPath();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("(concept!~client!value)", path);
        }
    }
}
