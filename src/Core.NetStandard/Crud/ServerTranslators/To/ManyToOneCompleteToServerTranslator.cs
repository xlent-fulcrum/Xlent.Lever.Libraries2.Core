using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.Crud.ServerTranslators.To
{
    /// <summary>
    /// Translate concept values to the server
    /// </summary>
    public class ManyToOneCompleteToServerTranslator<TModel> : ManyToOneCompleteToServerTranslator<TModel, TModel>, IManyToOneComplete<TModel, string>
    {

        /// <inheritdoc />
        public ManyToOneCompleteToServerTranslator(IManyToOneComplete<TModel, string> storage, string idConceptName,
            System.Func<string> getServerNameMethod, ITranslatorService translatorService)
            : base(storage, idConceptName, getServerNameMethod, translatorService)
        {
        }
    }

    /// <summary>
    /// Translate concept values to the server
    /// </summary>
    public class ManyToOneCompleteToServerTranslator<TModelCreate, TModel> : CrudToServerTranslator<TModelCreate, TModel>, IManyToOneComplete<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly IManyToOneComplete<TModelCreate, TModel, string> _storage;

        /// <inheritdoc />
        public ManyToOneCompleteToServerTranslator(IManyToOneComplete<TModelCreate, TModel, string> storage, string idConceptName, System.Func<string> getServerNameMethod, ITranslatorService translatorService)
            : base(storage, idConceptName, getServerNameMethod, translatorService)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null,
        CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(parentId).ExecuteAsync();
            parentId = translator.Translate(parentId);
            return await _storage.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> ReadChildrenAsync(string parentId, int limit = int.MaxValue, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(parentId).ExecuteAsync();
            parentId = translator.Translate(parentId);
            return await _storage.ReadChildrenAsync(parentId, limit, token);
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(string parentId, CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            await translator.Add(parentId).ExecuteAsync();
            parentId = translator.Translate(parentId);
            await _storage.DeleteChildrenAsync(parentId, token);
        }
    }
}