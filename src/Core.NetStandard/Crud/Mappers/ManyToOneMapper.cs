using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.PassThrough;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc cref="ManyToOneMapper{TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId}" />
    public class ManyToOneMapper<TClientModel, TClientId, TServerModel, TServerId> :
        ManyToOneMapper<TClientModel, TClientModel, TClientId, TServerModel, TServerId>,
        IManyToOne<TClientModel, TClientId>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ManyToOneMapper(IManyToOne<TServerModel, TServerId> storage, ICrudMapper<TClientModel, TServerModel> mapper)
            : base(storage, mapper)
        {
        }
    }

    /// <inheritdoc cref="IManyToOne{TManyModel,TClientId}" />
    public class ManyToOneMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId> : IManyToOne<TClientModel, TClientId> where TClientModel : TClientModelCreate
    {
        private readonly IManyToOne<TServerModel, TServerId> _storage;
        private readonly ICrudMapper<TClientModelCreate, TClientModel, TServerModel> _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        public ManyToOneMapper(IManyToOne<TServerModel, TServerId> storage, ICrudMapper<TClientModelCreate, TClientModel, TServerModel> mapper)
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
        public virtual async Task<IEnumerable<TClientModel>> ReadChildrenAsync(TClientId parentId, int limit = Int32.MaxValue, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(parentId);
            var items = await  _storage.ReadChildrenAsync(serverId, limit, token);
            FulcrumAssert.IsNotNull(items);
            return items?.Select(_mapper.MapFromServer);
        }
    }
}
