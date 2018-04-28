﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc cref="MapperBase{TClientModel,TClientId,TServerModel,TServerId}" />
    public class SlaveToMasterMapper<TClientModel, TClientId, TServerModel, TServerId> :
        SlaveToMasterMapper<TClientModel, TClientModel, TClientId, TServerModel, TServerId>,
        ISlaveToMasterComplete<TClientModel, TClientId>
    {
        /// <inheritdoc />
        public SlaveToMasterMapper(ISlaveToMasterComplete<TServerModel, TServerId> service,
            ISlaveToMasterModelMapper<TClientModel, TClientId, TServerModel>
                modelMapper)
            : base(service, modelMapper)
        {
        }
    }

    /// <inheritdoc cref="MapperBase{TClientModel,TClientId,TServerModel,TServerId}" />
    public class SlaveToMasterMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId> :
        RudMapper<TClientModel, SlaveToMasterId<TClientId>, TServerModel, SlaveToMasterId<TServerId>>,
        ISlaveToMasterComplete<TClientModelCreate, TClientModel, TClientId>
        where TClientModel : TClientModelCreate
    {
        private readonly ISlaveToMasterComplete<TServerModel, TServerId> _service;

        /// <summary>
        /// A mapping class that can map between the client and server model.
        /// </summary>
        public ISlaveToMasterModelMapper<TClientModelCreate, TClientModel, TClientId, TServerModel> SlaveToMasterModelMapper { get; }

        /// <inheritdoc />
        public SlaveToMasterMapper(ISlaveToMasterComplete<TServerModel, TServerId> service, ISlaveToMasterModelMapper<TClientModelCreate, TClientModel, TClientId, TServerModel> modelMapper)
            : base(service, modelMapper)
        {
            _service = service;
            SlaveToMasterModelMapper = modelMapper;
        }

        /// <inheritdoc />
        public async Task<SlaveToMasterId<TClientId>> CreateAsync(TClientId masterId, TClientModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var serverItem = await CreateInServerAndReturnAsync(masterId, item, token);
            var identifiable = serverItem as IUniquelyIdentifiable<SlaveToMasterId<TServerId>>;
            InternalContract.Require(identifiable != null,
                $"You can only call the method {nameof(CreateAsync)} if the type {typeof(TServerModel).FullName} implements {nameof(IUniquelyIdentifiable<TServerId>)}.");
            Debug.Assert(identifiable != null, nameof(identifiable) + " != null");
            return MapToClientId(identifiable.Id);
        }

        /// <inheritdoc />
        public async Task<TClientModel> CreateAndReturnAsync(TClientId masterId, TClientModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            var serverItem = await CreateInServerAndReturnAsync(masterId, item, token);
            return await MapFromServerAsync(serverItem, token);
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(SlaveToMasterId<TClientId> id, TClientModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            await CreateInServerAndReturnAsync(id, item, token);
        }

        /// <inheritdoc />
        public async Task<TClientModel> CreateWithSpecifiedIdAndReturnAsync(SlaveToMasterId<TClientId> id, TClientModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            var serverItem = await CreateInServerAndReturnAsync(id, item, token);
            return await MapFromServerAsync(serverItem, token);
        }

        /// <inheritdoc />
        public async Task<PageEnvelope<TClientModel>> ReadChildrenWithPagingAsync(TClientId masterId, int offset, int? limit = null,
            CancellationToken token = default(CancellationToken))
        {
            var serverMasterId = MapToServerId(masterId);
            var serverPage = await _service.ReadChildrenWithPagingAsync(serverMasterId, offset, limit, token);
            FulcrumAssert.IsNotNull(serverPage);
            return new PageEnvelope<TClientModel>(serverPage.PageInfo, await MapFromServerAsync(serverPage.Data, token));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TClientModel>> ReadChildrenAsync(TClientId masterId, int limit = Int32.MaxValue,
            CancellationToken token = default(CancellationToken))
        {
            var serverMasterId = MapToServerId(masterId);
            var serverItems = await _service.ReadChildrenAsync(serverMasterId, limit, token);
            return await MapFromServerAsync(serverItems, token);
        }

        /// <inheritdoc />
        public async Task DeleteChildrenAsync(TClientId masterId, CancellationToken token = default(CancellationToken))
        {
            var serverMasterId = MapToServerId(masterId);
            await _service.DeleteChildrenAsync(serverMasterId, token);
        }

        /// <summary>
        /// A convenience method to map a client <paramref name="id"/> to a a server id.
        /// </summary>
        protected static TServerId MapToServerId(TClientId id)
        {
            return MapperHelper.MapToType<TServerId, TClientId>(id);
        }

        /// <summary>
        /// A convenience method to map a <paramref name="clientItem"/> to a a server item.
        /// </summary>
        protected async Task<TServerModel> CreateInServerAndReturnAsync(TClientId masterId, TClientModelCreate clientItem, CancellationToken token = default(CancellationToken))
        {
            return await SlaveToMasterModelMapper.CreateAndReturnAsync(masterId, clientItem, token);
        }

        /// <summary>
        /// A convenience method to map a <paramref name="clientItem"/> to a a server item.
        /// </summary>
        protected async Task<TServerModel> CreateInServerAndReturnAsync(SlaveToMasterId<TClientId> masterId, TClientModelCreate clientItem, CancellationToken token = default(CancellationToken))
        {
            return await SlaveToMasterModelMapper.CreateWithSpecifiedIdAndReturnAsync(masterId, clientItem, token);
        }
    }
}