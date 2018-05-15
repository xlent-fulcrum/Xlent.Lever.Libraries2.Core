using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.Core.Crud.ServerTranslators.From
{
    /// <inheritdoc cref="CrdFromServerTranslator{TModelCreate, TModel}" />
    public class CrudFromServerTranslator<TModel> : CrudFromServerTranslator<TModel, TModel>, ICrud<TModel, string>
    {
        /// <inheritdoc />
        public CrudFromServerTranslator(ICrud<TModel, string> storage, string idConceptName,
            System.Func<string> getServerNameMethod)
            : base(storage, idConceptName, getServerNameMethod)
        {
        }
    }

    /// <inheritdoc cref="CrdFromServerTranslator{TModelCreate, TModel}" />
    public class CrudFromServerTranslator<TModelCreate, TModel> : CrdFromServerTranslator<TModelCreate, TModel>, ICrud<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly ICrud<TModelCreate, TModel, string> _storage;

        /// <inheritdoc />
        public CrudFromServerTranslator(ICrud<TModelCreate, TModel, string> storage, string idConceptName, System.Func<string> getServerNameMethod)
            : base(storage, idConceptName, getServerNameMethod)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(string id, TModel item, CancellationToken token = new CancellationToken())
        {
            await _storage.UpdateAsync(id, item, token);
        }

        /// <inheritdoc />
        public async Task<TModel> UpdateAndReturnAsync(string id, TModel item, CancellationToken token = new CancellationToken())
        {
            var result = await _storage.UpdateAndReturnAsync(id, item, token);
            var translator = CreateTranslator();
            return translator.DecorateItem(result);
        }
    }
}