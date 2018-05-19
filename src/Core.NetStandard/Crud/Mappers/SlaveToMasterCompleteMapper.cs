using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc cref="SlaveToMasterCrudMapper{TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId}" />
    public class SlaveToMasterCrudMapper<TClientModel, TClientId, TServerModel, TServerId> :
        SlaveToMasterCrudMapper<TClientModel, TClientModel, TClientId, TServerModel, TServerId>,
        ISlaveToMasterCrud<TClientModel, TClientId>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SlaveToMasterCrudMapper(ISlaveToMasterCrud<TServerModel, TServerId> storage,
            ICrudMapper<TClientModel, TServerModel> mapper)
            : base(storage, mapper)
        {
        }
    }

    /// <inheritdoc cref="ISlaveToMasterCrud{TManyModelCreate,TManyModel,TId}" />
    public class SlaveToMasterCrudMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId> : RudMapper<TClientModel, SlaveToMasterId<TClientId>, TServerModel, SlaveToMasterId<TServerId>>, ISlaveToMasterCrud<TClientModelCreate, TClientModel, TClientId> where TClientModel : TClientModelCreate
    {
        private readonly ISlaveToMasterCrud<TServerModel, TServerId> _storage;
        private readonly ICrudMapper<TClientModelCreate, TClientModel, TServerModel> _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        public SlaveToMasterCrudMapper(ISlaveToMasterCrud<TServerModel, TServerId> storage, ICrudMapper<TClientModelCreate, TClientModel, TServerModel> mapper)
        :base(storage, mapper)
        {
            _storage = storage;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public virtual async Task<PageEnvelope<TClientModel>> ReadChildrenWithPagingAsync(TClientId parentId, int offset, int? limit = null,
            CancellationToken token = default(CancellationToken))
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(parentId);
            var storagePage = await _storage.ReadChildrenWithPagingAsync(serverId, offset, limit, token);
            FulcrumAssert.IsNotNull(storagePage?.Data);
            var data = storagePage?.Data.Select(_mapper.MapFromServer);
            return new PageEnvelope<TClientModel>(storagePage?.PageInfo, data);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TClientModel>> ReadChildrenAsync(TClientId parentId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(parentId);
            var items = await _storage.ReadChildrenAsync(serverId, limit, token);
            FulcrumAssert.IsNotNull(items);
            return items?.Select(_mapper.MapFromServer);
        }

        /// <inheritdoc />
        public virtual async Task<SlaveToMasterId<TClientId>> CreateAsync(TClientId masterId, TClientModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var serverMasterId = MapperHelper.MapToType<TServerId, TClientId>(masterId);
            var record = _mapper.MapToServer(item);
            var serverId = await _storage.CreateAsync(serverMasterId, record, token);
            FulcrumAssert.IsNotDefaultValue(serverId);
            return MapperHelper.MapToType<TClientId, TServerId>(serverId);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> CreateAndReturnAsync(TClientId masterId, TClientModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var serverMasterId = MapperHelper.MapToType<TServerId, TClientId>(masterId);
            var record = _mapper.MapToServer(item);
            record = await _storage.CreateAndReturnAsync(serverMasterId, record, token);
            FulcrumAssert.IsNotDefaultValue(record);
            return _mapper.MapFromServer(record);
        }

        /// <inheritdoc />
        public virtual async Task CreateWithSpecifiedIdAsync(SlaveToMasterId<TClientId> id, TClientModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(id);
            var record = _mapper.MapToServer(item);
            await _storage.CreateWithSpecifiedIdAsync(serverId, record, token);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> CreateWithSpecifiedIdAndReturnAsync(SlaveToMasterId<TClientId> id, TClientModelCreate item,
            CancellationToken token = default(CancellationToken))
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(id);
            var record = _mapper.MapToServer(item);
            record = await _storage.CreateWithSpecifiedIdAndReturnAsync(serverId, record, token);
            return _mapper.MapFromServer(record);
        }

        /// <inheritdoc />
        public virtual Task DeleteChildrenAsync(TClientId parentId, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(parentId);
            return _storage.DeleteChildrenAsync(serverId, token);
        }
    }
}
