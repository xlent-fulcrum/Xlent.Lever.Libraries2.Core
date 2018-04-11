using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.MoveTo.Core.Mapping
{
    /// <summary>
    /// Map between two models.
    /// </summary>
    /// <typeparam name="TClientModel">The client model type.</typeparam>
    /// <typeparam name="TClientId">The client id type.</typeparam>
    /// <typeparam name="TServerLogic">The server logic to call (possibly with the server model and/or server id).</typeparam>
    /// <typeparam name="TServerModel">The server model type.</typeparam>
    /// <typeparam name="TServerId">The server id type.</typeparam>
    public abstract class MapperBase<TClientModel, TClientId, TServerLogic, TServerModel, TServerId>
    {
        /// <summary>
        /// The server logic to call (possibly with the server model and/or server id).
        /// </summary>
        protected TServerLogic ServerLogic { get; }

        /// <summary>
        /// A mapping class that can map between the client and server model.
        /// </summary>
        public IModelMapper<TClientModel, TServerLogic, TServerModel> ModelMapper { get; }

        /// <summary>
        /// Set up a new mapper between client and server types.
        /// </summary>
        /// <param name="serverLogic">The <see cref="ServerLogic"/></param>
        /// <param name="modelMapper">the <see cref="ModelMapper"/>.</param>
        protected MapperBase(TServerLogic serverLogic, IModelMapper<TClientModel, TServerLogic, TServerModel> modelMapper)
        {
            ServerLogic = serverLogic;
            ModelMapper = modelMapper;
        }

        /// <summary>
        /// A convenience method to map a list of <paramref name="serverItems"/> into a list of client items.
        /// </summary>
        protected async Task<TClientModel[]> CreateAndMapFromServerAsync(IEnumerable<TServerModel> serverItems, CancellationToken token = default(CancellationToken))
        {
            if (serverItems == null) return null;
            var clientItemTasks = serverItems.Select(async si => await CreateAndMapFromServerAsync(si, token));
            return await Task.WhenAll(clientItemTasks);
        }

        /// <summary>
        /// A convenience method to map a <paramref name="serverItem"/> to a a client item.
        /// </summary>

        protected async Task<TClientModel> CreateAndMapFromServerAsync(TServerModel serverItem, CancellationToken token = default(CancellationToken))
        {
            return await ModelMapper.CreateAndMapFromServerAsync(serverItem, ServerLogic, token);
        }

        /// <summary>
        /// A convenience method to map a <paramref name="clientItem"/> to a a server item.
        /// </summary>
        protected async Task<TServerModel> CreateAndMapToServerAsync(TClientModel clientItem, CancellationToken token = default(CancellationToken))
        {
            return await ModelMapper.CreateAndMapToServerAsync(clientItem, ServerLogic, token);
        }

        /// <summary>
        /// A convenience method to map a server <paramref name="id"/> to a a client id.
        /// </summary>
        protected static TClientId MapToClientId(TServerId id)
        {
            return MapperHelper.MapId<TClientId, TServerId>(id);
        }

        /// <summary>
        /// A convenience method to map a client <paramref name="id"/> to a a server id.
        /// </summary>
        protected static TServerId MapToServerId(TClientId id)
        {
            return MapperHelper.MapId<TServerId, TClientId>(id);
        }
    }
}
