using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.Crud.ClientTranslators
{
    /// <inheritdoc />
    public class CrdClientTranslator<TModelCreate, TModel> : ReadClientTranslator<TModel>, ICrd<TModelCreate, TModel, string>
    where TModel : IValidatable
    {
        private readonly ICrd<TModelCreate, TModel, string> _storage;

        /// <inheritdoc />
        public CrdClientTranslator(ICrd<TModelCreate, TModel, string> storage, string idConceptName, System.Func<string> getClientNameMethod, ITranslatorService translatorService)
        :base(storage, idConceptName, getClientNameMethod, translatorService)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<string> CreateAsync(TModelCreate item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            item = translator.DecorateItem(item);
            var decoratedId = await _storage.CreateAsync(item, token);
            // This is a new id, so there is no purpose in translating it.
            return decoratedId;
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TModelCreate item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            item = translator.DecorateItem(item);
            var decoratedResult = await _storage.CreateAndReturnAsync(item, token);
            await translator.Add(decoratedResult).ExecuteAsync();
            return translator.Translate(decoratedResult);
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(string id, TModelCreate item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            id = translator.Decorate(IdConceptName, id);
            item = translator.DecorateItem(item);
            await _storage.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(string id, TModelCreate item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            id = translator.Decorate(IdConceptName, id);
            item = translator.DecorateItem(item);
            var decoratedResult = await _storage.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
            await translator.Add(decoratedResult).ExecuteAsync();
            return translator.Translate(decoratedResult);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string id, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            id = translator.Decorate(IdConceptName, id);
            await _storage.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync(CancellationToken token = new CancellationToken())
        {
            await _storage.DeleteAllAsync(token);
        }
    }
}