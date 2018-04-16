﻿using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Mapping
{
    /// <inheritdoc cref="CrdMapper{TClientModelCreate,TClientModel,TClientId,TServerModel,TServerId}" />
    public class CrdMapper<TClientModel, TClientId, TServerModel, TServerId> : CrdMapper<TClientModel, TClientModel, TClientId, TServerModel, TServerId>,
        ICrd<TClientModel, TClientId>
        where TClientModel : IUniquelyIdentifiable<TClientId>
        where TServerModel : IUniquelyIdentifiable<TServerId>
    {
        /// <inheritdoc />
        public CrdMapper(ICrd<TServerModel, TServerModel, TServerId> service,
            IModelMapperWithCreate<TClientModel, TServerModel, TServerId> modelMapper)
            : base(service, modelMapper)
        {
        }
    }

    /// <inheritdoc cref="ReadMapper{TClientModel,TClientId,TServerModel,TServerId}" />
    public class CrdMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId> : 
        ReadMapper<TClientModel, TClientId, TServerModel, TServerId>, 
        ICrd<TClientModelCreate, TClientModel, TClientId>
        where TClientModel : TClientModelCreate, IUniquelyIdentifiable<TClientId>
        where TServerModel : IUniquelyIdentifiable<TServerId>
    {
        private readonly ICrd<TServerModel, TServerModel, TServerId> _service;

        /// <summary>
        /// A mapping class that can map between the client and server model.
        /// </summary>
        public new IModelMapperWithCreate<TClientModelCreate, TClientModel, TServerModel, TServerId> ModelMapper { get; }



        /// <inheritdoc />
        public CrdMapper(ICrd<TServerModel, TServerModel, TServerId> service, IModelMapperWithCreate<TClientModelCreate, TClientModel, TServerModel, TServerId> modelMapper)
            : base(service, modelMapper)
        {
            ModelMapper = modelMapper;
            _service = service;
        }

        /// <inheritdoc />
        public virtual async Task<TClientId> CreateAsync(TClientModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var serverItem = await CreateInServerAndReturnAsync(item, default(TServerId), token);
            return MapToClientId(serverItem.Id);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> CreateAndReturnAsync(TClientModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var serverItem = await CreateInServerAndReturnAsync(item, default(TServerId), token);
            return await MapFromServerAsync(serverItem, token);
        }

        /// <inheritdoc />
        public virtual async Task CreateWithSpecifiedIdAsync(TClientId id, TClientModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            await CreateInServerAndReturnAsync(item, serverId, token);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> CreateWithSpecifiedIdAndReturnAsync(TClientId id, TClientModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            var serverItem = await CreateInServerAndReturnAsync(item, serverId, token);
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

        /// <summary>
        /// A convenience method to map a <paramref name="clientItem"/> to a a server item.
        /// </summary>
        protected async Task<TServerModel> CreateInServerAndReturnAsync(TClientModelCreate clientItem, TServerId serverId = default(TServerId), CancellationToken token = default(CancellationToken))
        {
            return await ModelMapper.CreateInServerAndReturnAsync(clientItem, serverId, token);
        }
    }
}
