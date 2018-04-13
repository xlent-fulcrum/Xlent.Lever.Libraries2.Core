using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Mapping
{
    /// <inheritdoc cref="CrdMapper{TClientModel,TClientId,TServerLogic,TServerModel,TServerId}" />
    public class
        CrudMapper<TClientModel, TClientId, TServerLogic, TServerModel, TServerId> :
            CrdMapper<TClientModel, TClientId, TServerLogic, TServerModel, TServerId>,
            ICrud<TClientModel, TClientId> 
        where TClientModel : IUniquelyIdentifiable<TClientId>
        where TServerModel : IUniquelyIdentifiable<TServerId>
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
            var serverItem = await MapToServerAsync(item, token);
            await _service.UpdateAsync(serverId, serverItem, token);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> UpdateAndReturnAsync(TClientId id, TClientModel item, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            var serverItem = await MapToServerAsync(item, token);
            serverItem = await _service.UpdateAndReturnAsync(serverId, serverItem, token);
            return await MapFromServerAsync(serverItem, token);
        }
    }
}
