using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.ServerTranslators.From
{
    /// <summary>
    /// Decorate values from the server into concept values.
    /// </summary>
    public class SlaveToMasterFromServerTranslator<TModel> : SlaveToMasterFromServerTranslator<TModel, TModel>,
        ISlaveToMaster<TModel, string>
    {
        /// <inheritdoc />
        public SlaveToMasterFromServerTranslator(ISlaveToMasterCrud<TModel, string> storage,
            string masterIdConceptName, string slaveIdConceptName, System.Func<string> getServerNameMethod)
            : base(storage, masterIdConceptName, slaveIdConceptName, getServerNameMethod)
        {
        }
    }

    /// <summary>
    /// Decorate values from the server into concept values.
    /// </summary>
    public class SlaveToMasterFromServerTranslator<TModelCreate, TModel> : ServerTranslatorBase, ISlaveToMasterCrud<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly ISlaveToMasterCrud<TModelCreate, TModel, string> _storage;
        private readonly string _masterIdConceptName;

        /// <inheritdoc />
        public SlaveToMasterFromServerTranslator(ISlaveToMasterCrud<TModelCreate, TModel, string> storage, string masterIdConceptName, string slaveIdConceptName, System.Func<string> getServerNameMethod)
            : base(slaveIdConceptName, getServerNameMethod)
        {
            _storage = storage;
            _masterIdConceptName = masterIdConceptName;
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
        public async Task<SlaveToMasterId<string>> CreateAsync(string masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var result = await _storage.CreateAsync(masterId, item, token);
            var translator = CreateTranslator();
            return translator.Decorate(_masterIdConceptName, IdConceptName, result);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(string masterId, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var result = await _storage.CreateAndReturnAsync(masterId, item, token);
            var translator = CreateTranslator();
            FulcrumAssert.IsNotNull(result);
            return translator.DecorateItem(result);
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(SlaveToMasterId<string> id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            await _storage.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(SlaveToMasterId<string> id, TModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            var result = await _storage.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
            var translator = CreateTranslator();
            FulcrumAssert.IsNotNull(result);
            return translator.DecorateItem(result);
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(string masterId, CancellationToken token = default(CancellationToken))
        {
            await _storage.DeleteChildrenAsync(masterId, token);
        }

        /// <inheritdoc />
        public Task DeleteAsync(SlaveToMasterId<string> id, CancellationToken token = default(CancellationToken))
        {
            return _storage.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        public Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            return _storage.DeleteAllAsync(token);
        }
    }
}