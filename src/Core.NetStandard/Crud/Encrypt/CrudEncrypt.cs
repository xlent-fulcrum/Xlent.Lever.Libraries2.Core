using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.Core.Crud.Encrypt
{
    /// <inheritdoc cref="CrdEncrypt{TModel,TId}" />
    public class CrudEncrypt <TModel, TId>: CrdEncrypt<TModel, TId>, ICrud<TModel, TId>
    {
        private readonly ICrud<Storage.Logic.StorableAsByteArray<TModel, TId>, TId> _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="symmetricEncryptionKey"></param>
        public CrudEncrypt(ICrud<Storage.Logic.StorableAsByteArray<TModel, TId>, TId> storage, byte[] symmetricEncryptionKey)
        :base(storage, symmetricEncryptionKey)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(TId id, TModel item, CancellationToken token = new CancellationToken())
        {
            var storedItem = Encrypt(item);
            await _storage.UpdateAsync(id, storedItem, token);
        }

        /// <inheritdoc />
        public async Task<TModel> UpdateAndReturnAsync(TId id, TModel item, CancellationToken token = new CancellationToken())
        {
            var storedItem = Encrypt(item);
            storedItem = await _storage.UpdateAndReturnAsync(id, storedItem, token);
            return Decrypt(storedItem);
        }
    }
}