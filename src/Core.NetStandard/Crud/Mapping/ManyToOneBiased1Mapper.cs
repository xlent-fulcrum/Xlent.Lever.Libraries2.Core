using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Crud.Mapping
{
    /// <inheritdoc cref="MapperBase{TClientModel,TClientId,TServerLogic,TServerModel,TServerId}" />
    public class ManyToOneBiased1Mapper<TClientModel, TClientId, TServerLogic, TServerModel, TServerId> : MapperBase<TClientModel, TClientId, TServerLogic, TServerModel, TServerId>, IManyToOneRelation<TClientModel, TClientId>
    {
        private readonly IManyToManyBiased1<TServerModel, TServerId> _service;

        /// <inheritdoc />
        public ManyToOneBiased1Mapper(TServerLogic storage, IManyToManyBiased1<TServerModel, TServerId> service, IModelMapper<TClientModel, TServerLogic, TServerModel> modelMapper)
        : base(storage, modelMapper)
        {
            _service = service;
        }

        /// h<inheritdoc />
        public virtual async Task<PageEnvelope<TClientModel>> ReadChildrenWithPagingAsync(TClientId parentId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(parentId);
            var serverPage = await _service.ReadReferencedItemsByReference1WithPagingAsync(serverId, offset, limit, token);
            FulcrumAssert.IsNotNull(serverPage);
            return new PageEnvelope<TClientModel>(serverPage.PageInfo, await MapFromServerAsync(serverPage.Data, token));
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TClientModel>> ReadChildrenAsync(TClientId parentId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(parentId);
            var serverItems = await _service.ReadReferencedItemsByReference1Async(serverId, limit, token);
            return await MapFromServerAsync(serverItems, token);
        }

        /// <inheritdoc />
        public virtual async Task DeleteChildrenAsync(TClientId parentId, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(parentId);
            await _service.DeleteReferencedItemsByReference1(serverId, token);
        }
    }
}
