using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.MoveTo.Core.Translation;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.ServerTranslators.From
{
    /// <inheritdoc cref="CrdServerTranslatorFrom{TModel}" />
    public class CrudServerTranslatorFrom<TModel> : CrdServerTranslatorFrom<TModel>, ICrud<TModel, string>
    where TModel : IValidatable
    {
        private readonly ICrud<TModel, string> _storage;

        /// <inheritdoc />
        public CrudServerTranslatorFrom(ICrud<TModel, string> storage, string idConceptName, System.Func<string> getServerNameMethod)
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