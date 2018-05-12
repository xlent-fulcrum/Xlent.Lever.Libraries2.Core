using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc />
    public class ReadMapper<TClientModel, TClientId, TServerModel, TServerId> : IRead<TClientModel, TClientId>
    {
        private readonly IRead<TServerModel, TServerId> _storage;
        private readonly IReadMapper<TClientModel, TServerModel> _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReadMapper(IRead<TServerModel, TServerId> storage, IReadMapper<TClientModel, TServerModel> mapper)
        {
            _storage = storage;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> ReadAsync(TClientId id, CancellationToken token = new CancellationToken())
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(id);
            var record = await _storage.ReadAsync(serverId, token);
            return _mapper.MapFromServer(record);
        }

        /// <inheritdoc />
        public virtual async Task<PageEnvelope<TClientModel>> ReadAllWithPagingAsync(int offset, int? limit = null, CancellationToken token = new CancellationToken())
        {
            var storagePage = await _storage.ReadAllWithPagingAsync(offset, limit, token);
            FulcrumAssert.IsNotNull(storagePage?.Data);
            var data = storagePage?.Data.Select(_mapper.MapFromServer);
            return new PageEnvelope<TClientModel>(storagePage?.PageInfo, data);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TClientModel>> ReadAllAsync(int limit = 2147483647, CancellationToken token = new CancellationToken())
        {
            var items = await _storage.ReadAllAsync(limit, token);
            FulcrumAssert.IsNotNull(items);
            return items?.Select(_mapper.MapFromServer);
        }
    }
}