using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Security;

namespace Xlent.Lever.Libraries2.Core.Crud.Encrypt
{
    /// <inheritdoc cref="ReadEncrypt{TModel,TId}" />
    public class CrdEncrypt <TModel, TId>: ReadEncrypt<TModel, TId>, ICrd<TModel, TId>
    {
        private readonly SymmetricCrypto _symmetricCrypto;
        private readonly ICrd<Libraries2.Core.Storage.Logic.StorableAsByteArray<TModel, TId>, TId> _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="symmetricEncryptionKey"></param>
        public CrdEncrypt(ICrd<Libraries2.Core.Storage.Logic.StorableAsByteArray<TModel, TId>, TId> storage, byte[] symmetricEncryptionKey)
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
        public async Task DeleteAsync(TId id, CancellationToken token = new CancellationToken())
        {
            await _storage.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync(CancellationToken token = new CancellationToken())
        {
            await _storage.DeleteAllAsync(token);
        }
    }
}