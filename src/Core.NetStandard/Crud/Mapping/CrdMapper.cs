using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Mapping
{
    /// <inheritdoc cref="ReadMapper{TClientModel,TClientId,TServerLogic,TServerModel,TServerId}" />
    public class CrdMapper<TClientModel, TClientId, TServerLogic, TServerModel, TServerId> : ReadMapper<TClientModel, TClientId, TServerLogic, TServerModel, TServerId>, ICrd<TClientModel, TClientId>
    where TClientModel : IUniquelyIdentifiable<TClientId>
    where TServerModel : IUniquelyIdentifiable<TServerId>
    {
        private readonly ICrd<TServerModel, TServerId> _service;

        /// <inheritdoc />
        public CrdMapper(TServerLogic serverLogic, ICrd<TServerModel, TServerId> service, IModelMapper<TClientModel, TServerLogic, TServerModel> modelMapper)
        : base(serverLogic, service, modelMapper)
        {
            _service = service;
        }

        /// <inheritdoc />
        public virtual async Task<TClientId> CreateAsync(TClientModel item, CancellationToken token = default(CancellationToken))
        {
            var serverItem = MapToServer(item);
            var serverId = await _service.CreateAsync(serverItem, token);
            var clientId = MapToClientId(serverId);
            item.Id = clientId;
            await MapToServerAsync(item, token);
            return clientId;
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> CreateAndReturnAsync(TClientModel item, CancellationToken token = default(CancellationToken))
        {
            var serverItem = MapToServer(item);
            serverItem = await _service.CreateAndReturnAsync(serverItem, token);
            item.Id = MapToClientId(serverItem.Id);
            await MapToServerAsync(item, token);
            return await MapFromServerAsync(serverItem, token);
        }

        /// <inheritdoc />
        public virtual async Task CreateWithSpecifiedIdAsync(TClientId id, TClientModel item, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            var serverItem = await MapToServerAsync(item, token);
            await _service.CreateWithSpecifiedIdAsync(serverId, serverItem, token);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> CreateWithSpecifiedIdAndReturnAsync(TClientId id, TClientModel item, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            var serverItem = await MapToServerAsync(item, token);
            serverItem = await _service.CreateWithSpecifiedIdAndReturnAsync(serverId, serverItem, token);
            return await MapFromServerAsync(serverItem, token);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TClientId id, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            await _service.DeleteAsync(serverId, token);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            await _service.DeleteAllAsync(token);
        }
    }
}
