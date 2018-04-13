using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Mapping
{
    /// <inheritdoc cref="MapperBase{TClientModel,TClientId,TServerLogic,TServerModel,TServerId}" />
    public class ReadMapper<TClientModel, TClientId, TServerLogic, TServerModel, TServerId> : MapperBase<TClientModel, TClientId, TServerLogic, TServerModel, TServerId>, IReadAll<TClientModel, TClientId>
    {
        private readonly IReadAll<TServerModel, TServerId> _service;

        /// <inheritdoc />
        public ReadMapper(TServerLogic serverLogic, IReadAll<TServerModel, TServerId> service, IModelMapper<TClientModel, TServerLogic, TServerModel> modelMapper)
        : base(serverLogic, modelMapper)
        {
            _service = service;
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> ReadAsync(TClientId id, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            var serverItem = await _service.ReadAsync(serverId, token);
            return await MapFromServerAsync(serverItem, token);
        }

        /// <inheritdoc />
        public virtual async Task<PageEnvelope<TClientModel>> ReadAllWithPagingAsync(int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            var serverPage = await _service.ReadAllWithPagingAsync(offset, limit, token);
            FulcrumAssert.IsNotNull(serverPage);
            return new PageEnvelope<TClientModel>(serverPage.PageInfo, await MapFromServerAsync(serverPage.Data, token));
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TClientModel>> ReadAllAsync(int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            var serverItems = await _service.ReadAllAsync(limit, token);
            return await MapFromServerAsync(serverItems, token);
        }
    }
}
