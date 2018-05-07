using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Crud.ServerTranslators.Support
{
    internal class TestModelCreate
    {
        public const string StatusConceptName = "testmodel.status";
        public string Name { get; set; }

        [TranslationConcept("testmodel.status")]
        public string Status { get; set; }
    }
}