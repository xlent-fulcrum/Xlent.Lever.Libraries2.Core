using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.ServerTranslators.From
{
    /// <summary>
    /// Decorate values from the server into concept values.
    /// </summary>
    public class ManyToOneCompleteFromServerTranslator<TModel> : ManyToOneCompleteFromServerTranslator<TModel, TModel>,
        IManyToOneComplete<TModel, string>
    {
        /// <inheritdoc />
        public ManyToOneCompleteFromServerTranslator(IManyToOneComplete<TModel, string> storage, string idConceptName,
            System.Func<string> getServerNameMethod)
            : base(storage, idConceptName, getServerNameMethod)
        {
        }
    }

    /// <summary>
    /// Decorate values from the server into concept values.
    /// </summary>
    public class ManyToOneCompleteFromServerTranslator<TModelCreate, TModel> : CrudFromServerTranslator<TModelCreate, TModel>, IManyToOneComplete<TModelCreate, TModel, string> 
        where TModel : TModelCreate
    {
        private readonly IManyToOneComplete<TModelCreate, TModel, string> _storage;

        /// <inheritdoc />
        public ManyToOneCompleteFromServerTranslator(IManyToOneComplete<TModelCreate, TModel, string> storage, string idConceptName, System.Func<string> getServerNameMethod)
            : base(storage, idConceptName, getServerNameMethod)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null,
        CancellationToken token = new CancellationToken())
        {
            var result = await _storage.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
            var translator = CreateTranslator();
            return translator.DecorateItem(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> ReadChildrenAsync(string parentId, int limit = int.MaxValue, CancellationToken token = new CancellationToken())
        {
            var result = await _storage.ReadChildrenAsync(parentId, limit, token);
            var translator = CreateTranslator();
            return translator.DecorateItems(result);
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(string parentId, CancellationToken token = default(CancellationToken))
        {
            await _storage.DeleteChildrenAsync(parentId, token);
        }
    }
}