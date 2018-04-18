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
        /// Set up a new mapper between client and server types.
        /// </summary>
        protected MapperBase()
        {
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
