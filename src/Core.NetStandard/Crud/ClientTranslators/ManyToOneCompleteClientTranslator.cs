using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.Crud.ClientTranslators
{
    /// <inheritdoc cref="CrudClientTranslator{TModel}" />
    public class ManyToOneCompleteClientTranslator<TModel> : ManyToOneCompleteClientTranslator<TModel, TModel>,
        IManyToOneComplete<TModel, string>
    {
        /// <inheritdoc />
        public ManyToOneCompleteClientTranslator(IManyToOneComplete<TModel, string> storage,
            string parentIdConceptName, string idConceptName, System.Func<string> getClientNameMethod, ITranslatorService translatorService)
            : base(storage, parentIdConceptName, idConceptName, getClientNameMethod, translatorService)
        {
        }
    }

    /// <inheritdoc cref="CrudClientTranslator{TModel}" />
    public class ManyToOneCompleteClientTranslator<TModelCreate, TModel> : CrudClientTranslator<TModelCreate, TModel>, IManyToOneComplete<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly IManyToOneComplete<TModelCreate, TModel, string> _storage;
        private readonly string _parentIdConceptName;

        /// <inheritdoc />
        public ManyToOneCompleteClientTranslator(IManyToOneComplete<TModelCreate, TModel, string> storage, string parentIdConceptName, string idConceptName, System.Func<string> getClientNameMethod, ITranslatorService translatorService)
            : base(storage, idConceptName, getClientNameMethod, translatorService)
        {
            _storage = storage;
            _parentIdConceptName = parentIdConceptName;
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null,
        CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            parentId = translator.Decorate(_parentIdConceptName, parentId);
            var result = await _storage.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
            await translator.Add(result).ExecuteAsync();
            return translator.Translate(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> ReadChildrenAsync(string parentId, int limit = int.MaxValue, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            parentId = translator.Decorate(_parentIdConceptName, parentId);
            var result = await _storage.ReadChildrenAsync(parentId, limit, token);
            var array = result as TModel[] ?? result.ToArray();
            await translator.Add(array).ExecuteAsync();
            return translator.Translate(array);
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(string masterId, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            masterId = translator.Decorate(_parentIdConceptName, masterId);
            await _storage.DeleteChildrenAsync(masterId, token);
        }
    }
}