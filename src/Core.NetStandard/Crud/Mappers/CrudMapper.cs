using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc cref="CrudMapper{TClientModelCreate,TClientModel,TClientId,TServerModel,TServerId}" />
    public class
        CrudMapper<TClientModel, TClientId, TServerModel, TServerId> : CrudMapper<TClientModel, TClientModel, TClientId, TServerModel, TServerId>, ICrud<TClientModel, TClientId>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CrudMapper(ICrud<TServerModel, TServerId> storage,
            ICrudMapper<TClientModel, TServerModel> mapper)
        :base(storage, mapper)
        {
        }
    }

    /// <inheritdoc cref="ICrud{TModel,TId}" />
    public class CrudMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId> : CrdMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId>, ICrud<TClientModelCreate, TClientModel, TClientId>
        where TClientModel : TClientModelCreate
    {
        private readonly ICrud<TServerModel, TServerId> _storage;
        private readonly ICrudMapper<TClientModelCreate, TClientModel, TServerModel> _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        public CrudMapper(ICrud<TServerModel, TServerId> storage, ICrudMapper<TClientModelCreate, TClientModel, TServerModel> mapper)
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
    }
}