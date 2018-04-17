using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc cref="ReadMapper{TClientModel,TClientId,TServerModel,TServerId}" />
    public class RudMapper<TClientModel, TClientId, TServerModel, TServerId> : 
        ReadMapper<TClientModel, TClientId, TServerModel, TServerId>, 
        IRud<TClientModel, TClientId>
    {
        private readonly IRud<TServerModel, TServerId> _service;

        /// <inheritdoc />
        public RudMapper(IRud<TServerModel, TServerId> service, IModelMapper<TClientModel, TServerModel> modelMapper)
            : base(service, modelMapper)
        {
            _service = service;
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TClientId id, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            var serverId = MapToServerId(id);
            await _service.DeleteAsync(serverId, token);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            await _service.DeleteAllAsync(token);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(TClientId id, TClientModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotNull(item, nameof(item));
            var serverId = MapToServerId(id);
            var serverItem = await MapToServerAsync(item, token);
            await _service.UpdateAsync(serverId, serverItem, token);
        }

        /// <inheritdoc />
        public async Task<TClientModel> UpdateAndReturnAsync(TClientId id, TClientModel item, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            InternalContract.RequireNotNull(item, nameof(item));
            var serverId = MapToServerId(id);
            var serverItem = await MapToServerAsync(item, token);
            serverItem = await _service.UpdateAndReturnAsync(serverId, serverItem, token);
            return await MapFromServerAsync(serverItem, token);
        }
    }
}
