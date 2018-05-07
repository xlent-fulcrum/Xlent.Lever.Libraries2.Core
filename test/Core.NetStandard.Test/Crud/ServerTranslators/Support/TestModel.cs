using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.NetFramework.Test.Core.Crud.ServerTranslators.Support
{
    internal class TestModel : TestModelCreate, IUniquelyIdentifiable<string>, IOptimisticConcurrencyControlByETag
    {
        public const string IdConceptName = "testmodel.id";

        /// <inheritdoc />
        [TranslationConcept("testmodel.id")]
        public string Id { get; set; }

        /// <inheritdoc />
        public string Etag { get; set; }
    }
}