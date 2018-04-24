using System.IO;
using System.Security.Cryptography;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.Core.Security
{
    /// <summary>
    /// Class to encrypt or decrypt messages
    /// </summary>
    public class SymmetricCrypto
    {
        private readonly byte[] _symmetricEncryptionKey;
        private readonly byte[] _iv;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symmetricEncryptionKey">The symmetric key to use both to encrypt and to decrypt.</param>
        /// <param name="iv">. Will be used when you call the Encrypt/Decrypt methods without the iv parameter.</param>
        public SymmetricCrypto(byte[] symmetricEncryptionKey)
        {
            _symmetricEncryptionKey = symmetricEncryptionKey;
            _iv = iv;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symmetricEncryptionKey">The symmetric key to use both to encrypt and to decrypt.</param>
        /// <param name="iv">Will be used when you call the Encrypt/Decrypt methods without the iv parameter. Use null if you want a unique iv to be generated.</param>
        public SymmetricCrypto(byte[] symmetricEncryptionKey, ref byte[] iv)
        {
            _symmetricEncryptionKey = symmetricEncryptionKey;
            if (iv == null)
            {

            }
            _iv = iv;
        }

        /// <summary>
        /// Encrypt <paramref name="rawPlaintext"/>, using the symmetric key.
        /// <param name="rawPlaintext">The text to be encrypted.</param>
        /// <param name="iv">A unique key for this translation. Use null if you want a unique iv to be generated.</param>
        /// </summary>
        public byte[] Encrypt(byte[] rawPlaintext, ref byte[] iv)
        {
            return ConvertText(rawPlaintext, ref iv);
        }

        /// <summary>
        /// Encrypt <paramref name="rawPlaintext"/>, using the symmetric key.
        /// <param name="rawPlaintext">The text to be encrypted.</param>
        /// </summary>
        public byte[] Encrypt(byte[] rawPlaintext)
        {
            InternalContract.Require(_iv != null, );
            return ConvertText(rawPlaintext, ref iv);
        }

        /// <summary>
        /// Decrypt <paramref name="cipherText"/>, using the symmetric key.
        /// <param name="iv"></param>
        /// </summary>
        public byte[] Decrypt(byte[] cipherText, byte[] iv)
        {
            InternalContract.RequireNotNull(iv, nameof(iv));
            return ConvertText(cipherText, ref iv);
        }

        private byte[] ConvertText(byte[] text, ref byte[] iv)
        {
            if (text == null) return null;
            using (Aes aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = _symmetricEncryptionKey.Length * 8;
                aes.Key = _symmetricEncryptionKey;
                if (iv != null) aes.IV = iv;
                else
                {
                    aes.GenerateIV();
                    iv = aes.IV;
                }
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(text, 0, text.Length);
                    }

                    return ms.ToArray();
                }
            }
        }

        private byte[] GenerateIv(int keySizeInBytes)
        {
            using (Aes aes = new AesManaged())
            {
                aes.KeySize = keySizeInBytes * 8;

                    aes.GenerateIV();
                    iv = aes.IV;
                }
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(text, 0, text.Length);
                    }

                    return ms.ToArray();
                }
            }
        }
    }
}
