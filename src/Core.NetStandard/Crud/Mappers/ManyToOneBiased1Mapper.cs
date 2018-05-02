using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc cref="MapperBase{TClientModel,TClientId,TServerModel,TServerId}" />
    public class ManyToOneBiased1Mapper<TClientModel, TClientId, TServerModel, TServerId> : MapperBase<TClientModel, TClientId, TServerModel, TServerId>, IManyToOne<TClientModel, TClientId>
    {
        private readonly IManyToManyBiased1<TServerModel, TServerId> _service;

        /// <summary>
        /// A mapping class that can map between the client and server model.
        /// </summary>
        public IReadModelMapper<TClientModel, TServerModel> ReadModelMapper { get; }

        /// <inheritdoc />
        public ManyToOneBiased1Mapper(IManyToManyBiased1<TServerModel, TServerId> service, IReadModelMapper<TClientModel, TServerModel> modelMapper)
        : base()
        {
            _service = service;
            ReadModelMapper = modelMapper;
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
        public virtual async Task DeleteChildrenAsync(TClientId masterId, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(masterId);
            await _service.DeleteReferencedItemsByReference1(serverId, token);
        }

        /// <summary>
        /// A convenience method to map a list of <paramref name="serverItems"/> into a list of client items.
        /// </summary>
        protected async Task<TClientModel[]> MapFromServerAsync(IEnumerable<TServerModel> serverItems, CancellationToken token = default(CancellationToken))
        {
            if (serverItems == null) return null;
            var clientItemTasks = serverItems.Select(async si => await MapFromServerAsync(si, token));
            return await Task.WhenAll(clientItemTasks);
        }

        /// <summary>
        /// A convenience method to map a <paramref name="serverItem"/> to a a client item.
        /// </summary>
        protected async Task<TClientModel> MapFromServerAsync(TServerModel serverItem, CancellationToken token = default(CancellationToken))
        {
            return await ReadModelMapper.MapFromServerAsync(serverItem, token);
        }
    }
}
