using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <summary>
    /// Map between two models.
    /// </summary>
    /// <typeparam name="TClientModel">The client model type.</typeparam>
    /// <typeparam name="TClientId">The client id type.</typeparam>
    /// <typeparam name="TServerModel">The server model type.</typeparam>
    /// <typeparam name="TServerId">The server id type.</typeparam>
    public abstract class MapperBase<TClientModel, TClientId, TServerModel, TServerId>
    {

        /// <summary>
        /// A mapping class that can map between the client and server model.
        /// </summary>
        public IModelMapper<TClientModel, TServerModel> ModelMapper { get; }

        /// <summary>
        /// Set up a new mapper between client and server types.
        /// </summary>
        /// <param name="modelMapper">the <see cref="ModelMapper"/>.</param>
        protected MapperBase(IModelMapper<TClientModel, TServerModel> modelMapper)
        {
            ModelMapper = modelMapper;
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
            return await ModelMapper.MapFromServerAsync(serverItem, token);
        }

        /// <summary>
        /// A convenience method to map a <paramref name="clientItem"/> to a a server item.
        /// </summary>
        protected async Task<TServerModel> MapToServerAsync(TClientModel clientItem, CancellationToken token = default(CancellationToken))
        {
            return await ModelMapper.MapToServerAsync(clientItem, token);
        }

        /// <summary>
        /// A convenience method to map a server <paramref name="id"/> to a a client id.
        /// </summary>
        protected static TClientId MapToClientId(TServerId id)
        {
            return MapperHelper.MapToType<TClientId, TServerId>(id);
        }

        /// <summary>
        /// A convenience method to map a client <paramref name="id"/> to a a server id.
        /// </summary>
        protected static TServerId MapToServerId(TClientId id)
        {
            return MapperHelper.MapToType<TServerId, TClientId>(id);
        }
    }
}
