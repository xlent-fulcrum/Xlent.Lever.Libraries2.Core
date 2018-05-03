using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Security;
using Xlent.Lever.Libraries2.Core.Storage.Logic;

namespace Xlent.Lever.Libraries2.Core.Crud.Encrypt
{
    /// <inheritdoc cref="ReadEncrypt{TModel,TId}" />
    public class CrdEncrypt <TModel, TId>: ReadEncrypt<TModel, TId>, ICrd<TModel, TId>
    {
        private readonly SymmetricCrypto _symmetricCrypto;
        private readonly ICrd<StorableAsByteArray<TModel, TId>, TId> _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="symmetricEncryptionKey"></param>
        public CrdEncrypt(ICrd<StorableAsByteArray<TModel, TId>, TId> storage, byte[] symmetricEncryptionKey)
            : base(storage, symmetricEncryptionKey)
        {
            _storage = storage;
            _symmetricCrypto = new SymmetricCrypto(symmetricEncryptionKey);
        }

        /// <inheritdoc />
        public async Task<TId> CreateAsync(TModel item, CancellationToken token = new CancellationToken())
        {
            var storedItem = Encrypt(item);
            return await _storage.CreateAsync(storedItem, token);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateAndReturnAsync(TModel item, CancellationToken token = new CancellationToken())
        {
            var storedItem = Encrypt(item);
            storedItem = await _storage.CreateAndReturnAsync(storedItem, token);
            return Decrypt(storedItem);
        }

        /// <inheritdoc />
        public Task DeleteAsync(TId id, CancellationToken token = new CancellationToken())
        {
            return _storage.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        public Task DeleteAllAsync(CancellationToken token = new CancellationToken())
        {
            return _storage.DeleteAllAsync(token);
        }

        /// <inheritdoc />
        public async Task CreateWithSpecifiedIdAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            var storedItem = Encrypt(item);
            await _storage.CreateWithSpecifiedIdAsync(id, storedItem, token);
        }

        /// <inheritdoc />
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            var storedItem = Encrypt(item);
            storedItem = await _storage.CreateWithSpecifiedIdAndReturnAsync(id, storedItem, token);
            return Decrypt(storedItem);
        }

        /// <inheritdoc />
        public Task<Lock> ClaimLockAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            return _storage.ClaimLockAsync(id, token);
        }

        /// <inheritdoc />
        public Task ReleaseLockAsync(Lock @lock, CancellationToken token = default(CancellationToken))
        {
            return _storage.ReleaseLockAsync(@lock, token);
        }
    }
}