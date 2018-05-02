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
    public class ReadMapper<TClientModel, TClientId, TServerModel, TServerId> : MapperBase<TClientModel, TClientId, TServerModel, TServerId>, IRead<TClientModel, TClientId>
    {
        private readonly IRead<TServerModel, TServerId> _service;

        /// <summary>
        /// A mapping class that can map between the client and server model.
        /// </summary>
        public IReadModelMapper<TClientModel, TServerModel> ReadModelMapper { get; }

        /// <inheritdoc />
        public ReadMapper(IRead<TServerModel, TServerId> service, IReadModelMapper<TClientModel, TServerModel> modelMapper)
        {
            _service = service;
            ReadModelMapper = modelMapper;
        }

        /// <inheritdoc />
        public virtual async Task<TClientModel> ReadAsync(TClientId id, CancellationToken token = default(CancellationToken))
        {
            var serverId = MapToServerId(id);
            var serverItem = await _service.ReadAsync(serverId, token);
            return await MapFromServerAsync(serverItem, token);
        }

        /// <inheritdoc />
        public virtual async Task<PageEnvelope<TClientModel>> ReadAllWithPagingAsync(int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            var serverPage = await _service.ReadAllWithPagingAsync(offset, limit, token);
            FulcrumAssert.IsNotNull(serverPage);
            return new PageEnvelope<TClientModel>(serverPage.PageInfo, await MapFromServerAsync(serverPage.Data, token));
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TClientModel>> ReadAllAsync(int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            var serverItems = await _service.ReadAllAsync(limit, token);
            return await MapFromServerAsync(serverItems, token);
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
