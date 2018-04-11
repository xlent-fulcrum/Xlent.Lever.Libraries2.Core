using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.MoveTo.Core.ServerTranslators
{
    /// <inheritdoc />
    public class CrudServerTranslator<TModel> : CrdServerTranslator<TModel>, ICrud<TModel, string>
    where TModel : IValidatable
    {
        private readonly ICrud<TModel, string> _storage;

        /// <inheritdoc />
        public CrudServerTranslator(ICrud<TModel, string> storage, string idConceptName, System.Func<string> getServerNameFunction, ITranslatorService translatorService)
        :base(storage, idConceptName, getServerNameFunction, translatorService)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(string id, TModel item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(id).Add(item).ExecuteAsync();
            id = translator.Translate(id);
            item = translator.Translate(item);
            await _storage.UpdateAsync(id, item, token);
        }

        /// <inheritdoc />
        public async Task<TModel> UpdateAndReturnAsync(string id, TModel item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(id).Add(item).ExecuteAsync();
            id = translator.Translate(id);
            item = translator.Translate(item);
            var result = await _storage.UpdateAndReturnAsync(id, item, token);
            return translator.DecorateItem(result);
        }
    }
}