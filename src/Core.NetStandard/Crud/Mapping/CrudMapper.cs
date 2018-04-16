using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Mapping
{
    /// <inheritdoc cref="CrudMapper{TClientModelCreate, TClientModel,TClientId,TServerModel,TServerId}" />
    public class
        CrudMapper<TClientModel, TClientId, TServerModel, TServerId> : CrudMapper<TClientModel, TClientModel, TClientId, TServerModel, TServerId>,
            ICrud<TClientModel, TClientId>
        where TClientModel : IUniquelyIdentifiable<TClientId>
        where TServerModel : IUniquelyIdentifiable<TServerId>
    {
        /// <inheritdoc />
        public CrudMapper(ICrud<TServerModel, TServerModel, TServerId> service,
            IModelMapperWithCreate<TClientModel, TServerModel, TServerId> modelMapper)
            : base(service, modelMapper)
        {
        }
    }

    /// <inheritdoc cref="CrdMapper{TClientModelCreate, TClientModel,TClientId,TServerModel,TServerId}" />
    public class
        CrudMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId> :
            CrdMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId>,
            ICrud<TClientModelCreate, TClientModel, TClientId>
        where TClientModel : TClientModelCreate, IUniquelyIdentifiable<TClientId>
        where TServerModel : IUniquelyIdentifiable<TServerId>
    {
        private readonly ICrud<TServerModel, TServerModel, TServerId> _service;

        /// <inheritdoc />
        public CrudMapper(ICrud<TServerModel, TServerModel, TServerId> service, IModelMapperWithCreate<TClientModelCreate, TClientModel, TServerModel, TServerId> modelMapper)
            : base(service, modelMapper)
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
