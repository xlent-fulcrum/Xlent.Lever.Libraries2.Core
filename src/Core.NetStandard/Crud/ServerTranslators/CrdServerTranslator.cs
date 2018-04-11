using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.MoveTo.Core.ServerTranslators
{
    /// <inheritdoc />
    public class CrdServerTranslator<TModel> : ReadServerTranslator<TModel>, ICrd<TModel, string>
    where TModel : IValidatable
    {
        private readonly ICrd<TModel, string> _storage;

        /// <inheritdoc />
        public CrdServerTranslator(ICrd<TModel, string> storage, string idConceptName, System.Func<string> getServerNameFunction, ITranslatorService translatorService)
        : base(storage, idConceptName, getServerNameFunction, translatorService)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<string> CreateAsync(TModel item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(item).ExecuteAsync();
            item = translator.Translate(item);
            var id = await _storage.CreateAsync(item, token);
            return translator.Decorate(IdConceptName, id);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TModel item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(item).ExecuteAsync();
            item = translator.Translate(item);
            var decoratedResult = await _storage.CreateAndReturnAsync(item, token);
            return translator.DecorateItem(decoratedResult);
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(string id, TModel item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(id).Add(item).ExecuteAsync();
            id = translator.Translate(id);
            item = translator.Translate(item);
            await _storage.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(string id, TModel item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(id).Add(item).ExecuteAsync();
            id = translator.Translate(id);
            item = translator.Translate(item);
            var result = await _storage.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
            return translator.DecorateItem(result);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string id, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(id).ExecuteAsync();
            id = translator.Translate(id);
            await _storage.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync(CancellationToken token = new CancellationToken())
        {
            await _storage.DeleteAllAsync(token);
        }
    }
}