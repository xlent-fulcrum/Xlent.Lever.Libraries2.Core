using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.Core.Crud.ServerTranslators.From
{
    /// <inheritdoc cref="CrdServerTranslatorFrom{TModelCreate, TModel}" />
    public class CrudServerTranslatorFrom<TModelCreate, TModel> : CrdServerTranslatorFrom<TModelCreate, TModel>, ICrud<TModelCreate, TModel, string>
    where TModel : IValidatable
    {
        private readonly ICrud<TModelCreate, TModel, string> _storage;

        /// <inheritdoc />
        public CrudServerTranslatorFrom(ICrud<TModelCreate, TModel, string> storage, string idConceptName, System.Func<string> getServerNameMethod)
        :base(storage, idConceptName, getServerNameMethod)
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