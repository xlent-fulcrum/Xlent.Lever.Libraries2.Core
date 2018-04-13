using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Mapping
{
    /// <inheritdoc cref="MapperBase{TClientModel,TClientId,TServerLogic,TServerModel,TServerId}" />
    public class ManyToOneMapper<TClientModel, TClientId, TServerLogic, TServerModel, TServerId> : MapperBase<TClientModel, TClientId, TServerLogic, TServerModel, TServerId>, IManyToOneRelation<TClientModel, TClientId>
    {
        private readonly IManyToOneRelation<TServerModel, TServerId> _service;

        /// <inheritdoc />
        public ManyToOneMapper(TServerLogic storage, IManyToOneRelation<TServerModel, TServerId> service, IModelMapper<TClientModel, TServerLogic, TServerModel> modelMapper)
        : base(storage, modelMapper)
        {
            _service = service;
        }

        /// h<inheritdoc />
        public virtual async Task<PageEnvelope<TClientModel>> ReadChildrenWithPagingAsync(TClientId parentId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(parentId);
            var serverPage = await _service.ReadChildrenWithPagingAsync(serverId, offset, limit, token);
            FulcrumAssert.IsNotNull(serverPage);
            return new PageEnvelope<TClientModel>(serverPage.PageInfo, await MapFromServerAsync(serverPage.Data, token));
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TClientModel>> ReadChildrenAsync(TClientId parentId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(parentId);
            var serverItems = await _service.ReadChildrenAsync(serverId, limit, token);
            return await MapFromServerAsync(serverItems, token);
        }

        /// <inheritdoc />
        public virtual async Task DeleteChildrenAsync(TClientId parentId, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(parentId);
            await _service.DeleteChildrenAsync(serverId, token);
        }
    }
}
