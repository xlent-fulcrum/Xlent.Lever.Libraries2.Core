using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.Crud.ServerTranslators.To
{
    /// <inheritdoc cref="ReadServerTranslatorTo{TModel}" />
    public class CrdServerTranslatorTo<TModel> : CrdServerTranslatorTo<TModel, TModel>, ICrd<TModel, string>
    {
        /// <inheritdoc />
        public CrdServerTranslatorTo(ICrd<TModel, string> storage, string idConceptName,
            System.Func<string> getServerNameMethod, ITranslatorService translatorService)
            : base(storage, idConceptName, getServerNameMethod, translatorService)
        {
        }
    }

    /// <inheritdoc cref="ReadServerTranslatorTo{TModel}" />
    public class CrdServerTranslatorTo<TModelCreate, TModel> : ReadServerTranslatorTo<TModel>, ICrd<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly ICrd<TModelCreate, TModel, string> _storage;

        /// <inheritdoc />
        public CrdServerTranslatorTo(ICrd<TModelCreate, TModel, string> storage, string idConceptName, System.Func<string> getServerNameMethod, ITranslatorService translatorService)
            : base(storage, idConceptName, getServerNameMethod, translatorService)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<string> CreateAsync(TModelCreate item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(item).ExecuteAsync();
            item = translator.Translate(item);
            return await _storage.CreateAsync(item, token);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TModelCreate item, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(item).ExecuteAsync();
            item = translator.Translate(item);
            return await _storage.CreateAndReturnAsync(item, token);
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
        public Task DeleteAllAsync(CancellationToken token = new CancellationToken())
        {
            return _storage.DeleteAllAsync(token);
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(string id, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            await translator.Add(id).Add(item).ExecuteAsync();
            id = translator.Translate(id);
            item = translator.Translate(item);
            await _storage.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(string id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            await translator.Add(id).Add(item).ExecuteAsync();
            id = translator.Translate(id);
            item = translator.Translate(item);
            return await _storage.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
        }

        /// <inheritdoc />
        public async Task<Lock> ClaimLockAsync(string id, CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            await translator.Add(id).ExecuteAsync();
            id = translator.Translate(id);
            return await _storage.ClaimLockAsync(id, token);
        }

        /// <inheritdoc />
        public Task ReleaseLockAsync(Lock @lock, CancellationToken token = default(CancellationToken))
        {
            return _storage.ReleaseLockAsync(@lock, token);
        }
    }
}