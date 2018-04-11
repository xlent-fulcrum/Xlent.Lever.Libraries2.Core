using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.MoveTo.Core.ServerTranslators
{
    /// <inheritdoc />
    public class ManyToOneServerTranslator<TModel> : ServerTranslatorBase, IManyToOneRelation<TModel, string>
    where TModel : IValidatable
    {
        private readonly IManyToOneRelation<TModel, string> _storage;

        /// <inheritdoc />
        public ManyToOneServerTranslator(IManyToOneRelation<TModel, string> storage, string idConceptName, System.Func<string> getServerNameFunction, ITranslatorService translatorService)
        :base(idConceptName, getServerNameFunction, translatorService)
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
            var result = await _storage.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
            return translator.DecorateItem(result);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> ReadChildrenAsync(string parentId, int limit = int.MaxValue, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(parentId).ExecuteAsync();
            parentId = translator.Translate(parentId);
            var result = await _storage.ReadChildrenAsync(parentId, limit, token);
            return translator.DecorateItems(result);
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(string parentId, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            await translator.Add(parentId).ExecuteAsync();
            parentId = translator.Translate(parentId);
            await _storage.DeleteChildrenAsync(parentId, token);
        }
    }
}