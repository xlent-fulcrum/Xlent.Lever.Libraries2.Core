using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Core.Translation;

namespace Xlent.Lever.Libraries2.Core.Crud.ClientTranslators
{
    /// <inheritdoc cref="ISlaveToMaster{TManyModel,TId}" />
    public class SlaveToMasterClientTranslator<TModel> : SlaveToMasterClientTranslator<TModel, TModel>,
        ISlaveToMaster<TModel, string>
    {
        /// <inheritdoc />
        public SlaveToMasterClientTranslator(ISlaveToMasterCrud<TModel, string> storage,
            string masterIdConceptName, string slaveIdConceptName, System.Func<string> getClientNameMethod,
            ITranslatorService translatorService)
            : base(storage, masterIdConceptName, slaveIdConceptName, getClientNameMethod, translatorService)
        {
        }
    }

    /// <inheritdoc cref="ISlaveToMaster{TManyModel,TId}" />
    public class SlaveToMasterClientTranslator<TModelCreate, TModel> : ClientTranslatorBase, ISlaveToMasterCrud<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly ISlaveToMasterCrud<TModelCreate, TModel, string> _storage;
        private readonly string _masterIdConceptName;

        /// <inheritdoc />
        public SlaveToMasterClientTranslator(ISlaveToMasterCrud<TModelCreate, TModel, string> storage, string masterIdConceptName, string slaveIdConceptName, System.Func<string> getClientNameMethod, ITranslatorService translatorService)
            : base(slaveIdConceptName, getClientNameMethod, translatorService)
        {
            _masterIdConceptName = masterIdConceptName;
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null,
        CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            parentId = translator.Decorate(_masterIdConceptName, parentId);
            var decoratedResult = await _storage.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
            await translator.Add(decoratedResult).ExecuteAsync();
            return translator.Translate(decoratedResult);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TModel>> ReadChildrenAsync(string parentId, int limit = int.MaxValue, CancellationToken token = new CancellationToken())
        {
            var translator = CreateTranslator();
            parentId = translator.Decorate(_masterIdConceptName, parentId);
            var decoratedResult = await _storage.ReadChildrenAsync(parentId, limit, token);
            var array = decoratedResult as TModel[] ?? decoratedResult.ToArray();
            await translator.Add(array).ExecuteAsync();
            return translator.Translate(array);
        }

        /// <inheritdoc />
        public async Task<SlaveToMasterId<string>> CreateAsync(string masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            masterId = translator.Decorate(_masterIdConceptName, masterId);
            item = translator.DecorateItem(item);
            var decoratedResult = await _storage.CreateAsync(masterId, item, token);
            await translator.Add(decoratedResult).ExecuteAsync();
            return translator.Translate(decoratedResult);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(string masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            masterId = translator.Decorate(_masterIdConceptName, masterId);
            item = translator.DecorateItem(item);
            var decoratedResult = await _storage.CreateAndReturnAsync(masterId, item, token);
            await translator.Add(decoratedResult).ExecuteAsync();
            return translator.Translate(decoratedResult);
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(SlaveToMasterId<string> id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            id.MasterId = translator.Decorate(_masterIdConceptName, id.MasterId);
            id.SlaveId = translator.Decorate(IdConceptName, id.SlaveId);
            item = translator.DecorateItem(item);
            await _storage.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(SlaveToMasterId<string> id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            id.MasterId = translator.Decorate(_masterIdConceptName, id.MasterId);
            id.SlaveId = translator.Decorate(IdConceptName, id.SlaveId);
            item = translator.DecorateItem(item);
            var decoratedResult = await _storage.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
            await translator.Add(decoratedResult).ExecuteAsync();
            return translator.Translate(decoratedResult);
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(string masterId, CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            masterId = translator.Decorate(_masterIdConceptName, masterId);
            await _storage.DeleteChildrenAsync(masterId, token);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(SlaveToMasterId<string> id, CancellationToken token = default(CancellationToken))
        {
            var translator = CreateTranslator();
            id = translator.Decorate(_masterIdConceptName, IdConceptName, id);
            await _storage.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            await _storage.DeleteAllAsync(token);
        }
    }
}