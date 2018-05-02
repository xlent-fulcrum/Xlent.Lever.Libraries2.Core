using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc cref="CrdMapper{TClientModelCreate,TClientModel,TClientId,TServerModel,TServerId}" />
    public class CrdMapper<TClientModel, TClientId, TServerModel, TServerId> : CrdMapper<TClientModel, TClientModel, TClientId, TServerModel, TServerId>,
        ICrd<TClientModel, TClientId>
    {
        /// <inheritdoc />
        public CrdMapper(ICrd<TServerModel, TServerModel, TServerId> service,
            ICrdModelMapper<TClientModel, TClientId, TServerModel> modelMapper)
            : base(service, modelMapper)
        {
        }
    }

    /// <inheritdoc cref="ReadMapper{TClientModel,TClientId,TServerModel,TServerId}" />
    public class CrdMapper<TClientModelCreate, TClientModel, TClientId, TServerModel, TServerId> : 
        ReadMapper<TClientModel, TClientId, TServerModel, TServerId>, 
        ICrd<TClientModelCreate, TClientModel, TClientId>
        where TClientModel : TClientModelCreate
    {
        private readonly ICrd<TServerModel, TServerModel, TServerId> _service;

        /// <summary>
        /// A mapping class that can map between the client and server model.
        /// </summary>
        public ICrdModelMapper<TClientModelCreate, TClientModel, TClientId, TServerModel> CrdModelMapper { get; }
    
        /// <inheritdoc />
        public CrdMapper(ICrd<TServerModel, TServerModel, TServerId> service, ICrdModelMapper<TClientModelCreate, TClientModel, TClientId, TServerModel> modelMapper)
            : base(service, modelMapper)
        {
            CrdModelMapper = modelMapper;
            _service = service;
        }

        /// <inheritdoc />
        public virtual async Task<TClientId> CreateAsync(TClientModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var serverItem = await CreateInServerAndReturnAsync(item, token);
            var identifiable = serverItem as IUniquelyIdentifiable<TServerId>;
            InternalContract.Require(identifiable != null,
                $"You can only call the method {nameof(CreateAsync)} if the type {typeof(TServerModel).FullName} implements {nameof(IUniquelyIdentifiable<TServerId>)}.");
            Debug.Assert(identifiable != null, nameof(identifiable) + " != null");
            return MapToClientId(identifiable.Id);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> CreateAndReturnAsync(TClientModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var serverItem = await CreateInServerAndReturnAsync(item, token);
            return await MapFromServerAsync(serverItem, token);
        }

        /// <inheritdoc />
        public virtual async Task CreateWithSpecifiedIdAsync(TClientId id, TClientModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            await CreateInServerAndReturnAsync(id,item , token);
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> CreateWithSpecifiedIdAndReturnAsync(TClientId id, TClientModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var serverItem = await CreateInServerAndReturnAsync(id, item, token);
            return await MapFromServerAsync(serverItem, token);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TClientId id, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            await _service.DeleteAsync(serverId, token);
        }

        /// <inheritdoc />
        public virtual Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            return _service.DeleteAllAsync(token);
        }

        /// <inheritdoc />
        public virtual async Task<Lock> ClaimLockAsync(TClientId id, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            return await _service.ClaimLockAsync(serverId, token);
        }

        /// <inheritdoc />
        public virtual Task ReleaseLockAsync(Lock @lock, CancellationToken token = default(CancellationToken))
        {
            return _service.ReleaseLockAsync(@lock, token);
        }

        /// <summary>
        /// A convenience method to map a <paramref name="clientItem"/> to a a server item.
        /// </summary>
        protected Task<TServerModel> CreateInServerAndReturnAsync(TClientModelCreate clientItem, CancellationToken token = default(CancellationToken))
        {
            return CrdModelMapper.CreateAndReturnAsync(clientItem, token);
        }

        /// <summary>
        /// A convenience method to map a <paramref name="clientItem"/> to a a server item.
        /// </summary>
        protected Task<TServerModel> CreateInServerAndReturnAsync(TClientId id, TClientModelCreate clientItem, CancellationToken token = default(CancellationToken))
        {
            return CrdModelMapper.CreateWithSpecifiedIdAndReturnAsync(id, clientItem, token);
        }
    }
}
