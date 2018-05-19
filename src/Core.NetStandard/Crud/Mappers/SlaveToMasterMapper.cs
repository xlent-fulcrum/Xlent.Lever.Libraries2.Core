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
    /// <inheritdoc cref="SlaveToMasterMapper{TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId}" />
    public class SlaveToMasterMapper<TClientModel, TClientId, TServerModel, TServerId> :
        SlaveToMasterMapper<TClientModel, TClientModel, TClientId, TServerModel, TServerId>,
        ISlaveToMaster<TClientModel, TClientId>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SlaveToMasterMapper(ISlaveToMasterCrud<TServerModel, TServerId> storage,
            ICrudMapper<TClientModel, TServerModel> mapper)
            : base(storage, mapper)
        {
        }
    }

    /// <inheritdoc cref="ISlaveToMasterCrud{TModelCreate, TModel,TId}" />
    public class SlaveToMasterMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId> :
        ManyToOneMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId>,
        ISlaveToMasterCrud<TClientModelCreate, TClientModel, TClientId> 
        where TClientModel : TClientModelCreate
    {
        private readonly ISlaveToMasterCrud<TServerModel, TServerId> _storage;
        private readonly ICrudMapper<TClientModelCreate, TClientModel, TServerModel> _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        public SlaveToMasterMapper(ISlaveToMasterCrud<TServerModel, TServerId> storage, ICrudMapper<TClientModelCreate, TClientModel, TServerModel> mapper)
            : base(storage, mapper)
        {
            _storage = storage;
            _mapper = mapper;
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

        /// <inheritdoc />
        public virtual Task DeleteAsync(SlaveToMasterId<TClientId> id, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(id);
            return _storage.DeleteAsync(serverId, token);
        }

        /// <inheritdoc />
        public virtual Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            return _storage.DeleteAllAsync(token);
        }
    }
}
