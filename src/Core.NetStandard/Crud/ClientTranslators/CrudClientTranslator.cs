using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.Crud.ClientTranslators
{
    /// <inheritdoc />
    public class CrudClientTranslator<TModelCreate, TModel> : CrdClientTranslator<TModelCreate, TModel>, ICrud<TModelCreate, TModel, string>
    where TModel : TModelCreate, IValidatable
    {
        private readonly ICrud<TModelCreate, TModel, string> _storage;

        /// <inheritdoc />
        public CrudClientTranslator(ICrud<TModelCreate, TModel, string> storage, string idConceptName, System.Func<string> getClientNameMethod, ITranslatorService translatorService)
        :base(storage, idConceptName, getClientNameMethod, translatorService)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(string id, TModel item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            id = translator.Decorate(IdConceptName, id);
            item = translator.DecorateItem(item);
            await _storage.UpdateAsync(id, item, token);
        }

        /// <inheritdoc />
        public async Task<TModel> UpdateAndReturnAsync(string id, TModel item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            id = translator.Decorate(IdConceptName, id);
            item = translator.DecorateItem(item);
            var decoratedResult = await _storage.UpdateAndReturnAsync(id, item, token);
            await translator.Add(decoratedResult).ExecuteAsync();
            return translator.Translate(decoratedResult);
        }
    }
}