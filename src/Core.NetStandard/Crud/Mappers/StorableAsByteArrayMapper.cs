using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Crud.Mappers
{
    /// <inheritdoc />
    public class StorableAsByteArrayMapper<TClientModel, TClientId, TServerModel, TServerId> :
        ICrudModelMapper<TClientModel, TClientId, TServerModel>
            where TServerModel : class, IStorableAsByteArray<TClientModel, TClientId>, new()
    {
        private readonly ICreate<TServerModel, TServerId> _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storage">The storage to use.</param>
        public StorableAsByteArrayMapper(ICreate<TServerModel, TServerId> storage)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task<TClientModel> MapFromServerAsync(TServerModel source, CancellationToken token = new CancellationToken())
        {
            InternalContract.RequireNotNull(source, nameof(source));
            return await Task.FromResult(source.Data);
        }

        /// <inheritdoc />
        public async Task<TServerModel> MapToServerAsync(TClientModel source, CancellationToken token = new CancellationToken())
        {
            InternalContract.RequireNotNull(source, nameof(source));
            var target = new TServerModel
            {
                Data = source
            };
            return await Task.FromResult(target);
        }

        /// <inheritdoc />
        public async Task<TServerModel> CreateAndReturnAsync(TClientModel source, CancellationToken token = new CancellationToken())
        {
            InternalContract.RequireNotNull(source, nameof(source));
            var target = await MapToServerAsync(source, token);
            return await _storage.CreateAndReturnAsync(target, token);
        }

        /// <inheritdoc />
        public async Task<TServerModel> CreateWithSpecifiedIdAndReturnAsync(TClientId id, TClientModel source,
            CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotNull(source, nameof(source));
            var serverId = MapperHelper.MapToType<TServerId, TClientId>(id);
            var target = await MapToServerAsync(source, token);
            return await _storage.CreateWithSpecifiedIdAndReturnAsync(serverId, target, token);
        }
    }
}