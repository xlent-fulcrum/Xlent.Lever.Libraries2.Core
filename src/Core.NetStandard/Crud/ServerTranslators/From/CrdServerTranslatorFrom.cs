using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.Core.Crud.ServerTranslators.From
{
    /// <inheritdoc cref="ReadServerTranslatorFrom{TModel}" />
    public class CrdServerTranslatorFrom<TModel> : CrdServerTranslatorFrom<TModel, TModel>, ICrd<TModel, string>
    {
        /// <inheritdoc />
        public CrdServerTranslatorFrom(ICrd<TModel, string> storage, string idConceptName,
            System.Func<string> getServerNameMethod)
            : base(storage, idConceptName, getServerNameMethod)
        {
        }
    }

    /// <inheritdoc cref="ReadServerTranslatorFrom{TModel}" />
    public class CrdServerTranslatorFrom<TModelCreate, TModel> : ReadServerTranslatorFrom<TModel>, ICrd<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly ICrd<TModelCreate, TModel, string> _storage;

        /// <inheritdoc />
        public CrdServerTranslatorFrom(ICrd<TModelCreate, TModel, string> storage, string idConceptName, System.Func<string> getServerNameMethod)
            : base(storage, idConceptName, getServerNameMethod)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<string> CreateAsync(TModelCreate item, CancellationToken token = new CancellationToken())
        {
            var id = await _storage.CreateAsync(item, token);
            var translator = CreateTranslator();
            return translator.Decorate(IdConceptName, id);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TModelCreate item, CancellationToken token = new CancellationToken())
        {
            var decoratedResult = await _storage.CreateAndReturnAsync(item, token);
            var translator = CreateTranslator();
            return translator.DecorateItem(decoratedResult);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string id, CancellationToken token = new CancellationToken())
        {
            await _storage.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync(CancellationToken token = new CancellationToken())
        {
            await _storage.DeleteAllAsync(token);
        }
    }
}