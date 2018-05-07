using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Crud.ServerTranslators.Support
{
    public class TestModelCreate
    {
        public const string StatusConceptName = "testmodel.status";

        [TranslationConcept("testmodel.status")]
        public string Status { get; set; }

        public static string DecoratedStatus(string name, string status) => $"({StatusConceptName}!~{name}!{status})";

        public string DecoratedStatus(string name) => DecoratedStatus(name, Status);
    }
}