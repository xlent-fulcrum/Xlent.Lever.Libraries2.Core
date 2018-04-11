using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Mapping
{
    /// <inheritdoc />
    public class CrudMapper<TClientModel, TClientId, TServerLogic, TServerModel, TServerId> : CrdMapper<TClientModel, TClientId, TServerLogic, TServerModel, TServerId>, ICrud<TClientModel, TClientId>
    {
        private readonly ICrud<TServerModel, TServerId> _service;

        /// <inheritdoc />
        public CrudMapper(TServerLogic serverLogic, ICrud<TServerModel, TServerId> service, IModelMapper<TClientModel, TServerLogic, TServerModel> modelMapper)
        : base(serverLogic, service, modelMapper)
        {
            _service = service;
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(TClientId id, TClientModel item, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            var serverItem = await CreateAndMapToServerAsync(item, token);
            await _service.UpdateAsync(serverId, serverItem, token);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> UpdateAndReturnAsync(TClientId id, TClientModel item, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            var serverItem = await CreateAndMapToServerAsync(item, token);
            serverItem = await _service.UpdateAndReturnAsync(serverId, serverItem, token);
            return await CreateAndMapFromServerAsync(serverItem, token);
        }
    }
}
