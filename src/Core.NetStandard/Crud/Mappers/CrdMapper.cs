using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc cref="CrdMapper{TClientModelCreate,TClientModel,TClientId,TServerModel,TServerId}" />
    public class
        CrdMapper<TClientModel, TClientId, TServerModel, TServerId> : CrdMapper<TClientModel, TClientModel, TClientId, TServerModel, TServerId>, ICrd<TClientModel, TClientId>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CrdMapper(ICrd<TServerModel, TServerId> storage, ICrdMapper<TClientModel, TServerModel> mapper)
        :base(storage, mapper)
        {
        }
    }

    /// <inheritdoc cref="ReadMapper{TClientModel,TClientId,TServerModel,TServerId}" />
    public class CrdMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId> : ReadMapper<TClientModel, TClientId, TServerModel, TServerId>, ICrd<TClientModelCreate, TClientModel, TClientId>
        where TClientModel : TClientModelCreate
    {
        private readonly ICrd<TServerModel, TServerId> _storage;
        private readonly ICrdMapper<TClientModelCreate, TClientModel, TServerModel> _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        public CrdMapper(ICrd<TServerModel, TServerId> storage, ICrdMapper<TClientModelCreate, TClientModel, TServerModel> mapper)
            : base(storage, mapper)
        {
            _storage = storage;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public virtual async Task<TClientId> CreateAsync(TClientModelCreate item, CancellationToken token = new CancellationToken())
        {
            var record = _mapper.MapToServer(item);
            var serverId = await _storage.CreateAsync(record, token);
            FulcrumAssert.IsNotDefaultValue(serverId);
            return MapperHelper.MapToType<TClientId, TServerId>(serverId);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> CreateAndReturnAsync(TClientModelCreate item, CancellationToken token = new CancellationToken())
        {
            var record = _mapper.MapToServer(item);
            record = await _storage.CreateAndReturnAsync(record, token);
            FulcrumAssert.IsNotDefaultValue(record);
            return _mapper.MapFromServer(record);
        }

        /// <inheritdoc />
        public virtual async Task CreateWithSpecifiedIdAsync(TClientId id, TClientModelCreate item, CancellationToken token = new CancellationToken())
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(id);
            var record = _mapper.MapToServer(item);
            await _storage.CreateWithSpecifiedIdAsync(serverId, record, token);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> CreateWithSpecifiedIdAndReturnAsync(TClientId id, TClientModelCreate item,
            CancellationToken token = new CancellationToken())
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(id);
            var record = _mapper.MapToServer(item);
            record = await _storage.CreateWithSpecifiedIdAndReturnAsync(serverId, record, token);
            return _mapper.MapFromServer(record);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TClientId id, CancellationToken token = new CancellationToken())
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(id);
            await _storage.DeleteAsync(serverId, token);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAllAsync(CancellationToken token = new CancellationToken())
        {
            await _storage.DeleteAllAsync(token);
        }

        /// <inheritdoc />
        public Task<Lock> ClaimLockAsync(TClientId id, CancellationToken token = new CancellationToken())
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(id);
            return _storage.ClaimLockAsync(serverId, token);
        }

        /// <inheritdoc />
        public Task ReleaseLockAsync(Lock @lock, CancellationToken token = new CancellationToken())
        {
            return _storage.ReleaseLockAsync(@lock, token);
        }
    }
}