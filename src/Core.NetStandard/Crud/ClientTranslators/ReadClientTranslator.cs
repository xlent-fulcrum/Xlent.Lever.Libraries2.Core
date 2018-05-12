using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.Crud.ClientTranslators
{
    /// <inheritdoc cref="IRead{TModel,TId}" />
    public class ReadClientTranslator<TModel> : ClientTranslatorBase, IRead<TModel, string>
    {
        private readonly IRead<TModel, string> _storage;

        /// <inheritdoc />
        public ReadClientTranslator(IRead<TModel, string> storage, string idConceptName, System.Func<string> getClientNameMethod, ITranslatorService translatorService)
        :base(idConceptName, getClientNameMethod, translatorService)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public virtual async Task<TModel> ReadAsync(string id, CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            id = translator.Decorate(IdConceptName, id);
            var decoratedResult = await _storage.ReadAsync(id, token);
            await translator.Add(decoratedResult).ExecuteAsync();
            return translator.Translate(decoratedResult);   
        }

        /// <inheritdoc />
        public virtual async Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            var decoratedResult =  await _storage.ReadAllWithPagingAsync(offset, limit, token);
            await translator.Add(decoratedResult).ExecuteAsync();
            return translator.Translate(decoratedResult);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            var decoratedResult = await _storage.ReadAllAsync(limit, token);
            var decoratedArray = decoratedResult as TModel[] ?? decoratedResult.ToArray();
            await translator.Add(decoratedArray).ExecuteAsync();
            return translator.Translate(decoratedArray);
        }
    }
}