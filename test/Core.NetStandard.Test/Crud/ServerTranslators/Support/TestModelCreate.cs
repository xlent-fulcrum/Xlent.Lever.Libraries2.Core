using Xlent.Lever.Libraries2.Core.Translation;
#pragma warning disable 659

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Crud.ServerTranslators.Support
{
    /// <summary>
    /// A model for testing decoration and translation
    /// </summary>
    public class TestModelCreate
    {
        public const string StatusConceptName = "testmodel.status";

        [TranslationConcept("testmodel.status")]
        public string Status { get; set; }

        public static string DecoratedStatus(string name, string status) => $"({StatusConceptName}!~{name}!{status})";

        public string DecoratedStatus(string name) => DecoratedStatus(name, Status);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is TestModelCreate testModel)) return false;
            return Equals(Status, testModel.Status);
        }
    }
}