using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc cref="CrudMapper{TClientModelCreate, TClientModel,TClientId,TServerModel,TServerId}" />
    public class
        CrudMapper<TClientModel, TClientId, TServerModel, TServerId> : CrudMapper<TClientModel, TClientModel, TClientId, TServerModel, TServerId>,
            ICrud<TClientModel, TClientId>
    {
        /// <inheritdoc />
        public CrudMapper(ICrud<TServerModel, TServerModel, TServerId> service,
            ICrudModelMapper<TClientModel, TClientId, TServerModel> modelMapper)
            : base(service, modelMapper)
        {
        }
    }

    /// <inheritdoc cref="CrdMapper{TClientModelCreate, TClientModel,TClientId,TServerModel,TServerId}" />
    public class
        CrudMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId> :
            CrdMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId>,
            ICrud<TClientModelCreate, TClientModel, TClientId>
        where TClientModel : TClientModelCreate
    {
        private readonly ICrud<TServerModel, TServerModel, TServerId> _service;

        /// <summary>
        /// A mapping class that can map between the client and server model.
        /// </summary>
        public ICrudModelMapper<TClientModelCreate, TClientModel, TClientId, TServerModel> CrudModelMapper { get; }

        /// <inheritdoc />
        public CrudMapper(ICrud<TServerModel, TServerModel, TServerId> service, ICrudModelMapper<TClientModelCreate, TClientModel, TClientId, TServerModel> modelMapper)
            : base(service, modelMapper)
        {
            _service = service;
            CrudModelMapper = modelMapper;
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

        /// <summary>
        /// A convenience method to map a <paramref name="clientItem"/> to a a server item.
        /// </summary>
        protected async Task<TServerModel> MapToServerAsync(TClientModel clientItem, CancellationToken token = default(CancellationToken))
        {
            return await CrudModelMapper.MapToServerAsync(clientItem, token);
        }
    }
}
