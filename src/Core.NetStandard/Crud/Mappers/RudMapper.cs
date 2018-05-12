using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc cref="ReadMapper{TClientModel,TClientId,TServerModel,TServerId}" />
    public class RudMapper<TClientModel, TClientId, TServerModel, TServerId> : ReadMapper<TClientModel, TClientId, TServerModel, TServerId>, IRud<TClientModel, TClientId>
    {
        private readonly IRud<TServerModel, TServerId> _storage;
        private readonly IRudMapper<TClientModel, TServerModel> _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        public RudMapper(IRud<TServerModel, TServerId> storage, IRudMapper<TClientModel, TServerModel> mapper)
            : base(storage, mapper)
        {
            _storage = storage;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(TClientId id, TClientModel item, CancellationToken token = new CancellationToken())
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(id);
            var record = _mapper.MapToServer(item);
            await _storage.UpdateAsync(serverId, record, token);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> UpdateAndReturnAsync(TClientId id, TClientModel item, CancellationToken token = new CancellationToken())
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(id);
            var record = _mapper.MapToServer(item);
            record = await _storage.UpdateAndReturnAsync(serverId, record, token);
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