using System.Text;
using Newtonsoft.Json;
using Xlent.Lever.AsyncCaller2.Bll;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.Core.Storage.Logic
{
    /// <summary>
    /// The headers of a request
    /// </summary>
    public class StorableAsEncryptedByteArray<TData, TId> : StorableAsByteArray<TData, TId>
    {
        private readonly SymmetricCrypto _symmetricCrypto;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symmetricEncryptionKey">The symmetric key to use both to encrypt and to decrypt.</param>
        public StorableAsEncryptedByteArray(byte[] symmetricEncryptionKey)
        {
            _symmetricCrypto = new SymmetricCrypto(symmetricEncryptionKey);
        }

        /// <summary>
        /// This property is stored serialized as an encrypted byte array
        /// </summary>
        public override TData Data
        {
            get
            {
                var decrypted = _symmetricCrypto.Decrypt(ByteArray);
                if (typeof(TData) == decrypted.GetType()) return (TData)(object)decrypted;
                var jsonString = Encoding.UTF8.GetString(decrypted);
                return JsonConvert.DeserializeObject<TData>(jsonString);
            }
            set
            {
                byte[] valueAsBytes;
                if (typeof(TData) == ByteArray.GetType())
                {
                    valueAsBytes = (byte[]) (object) value;
                }
                else
                {
                    var jsonString = JsonConvert.SerializeObject(value);
                    valueAsBytes = Encoding.UTF8.GetBytes(jsonString);
                }
                ByteArray = _symmetricCrypto.Encrypt(valueAsBytes);
            }
        }
    }
}
