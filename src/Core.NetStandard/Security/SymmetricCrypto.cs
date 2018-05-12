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
        private byte[] _iv;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symmetricEncryptionKey">The symmetric key to use both to encrypt and to decrypt.</param>
        public SymmetricCrypto(byte[] symmetricEncryptionKey)
        {
            _symmetricEncryptionKey = symmetricEncryptionKey;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symmetricEncryptionKey">The symmetric key to use both to encrypt and to decrypt.</param>
        /// <param name="iv">Will be used when you call the Encrypt/Decrypt methods without the iv parameter. Use null if you want a unique iv to be generated.</param>
        public SymmetricCrypto(byte[] symmetricEncryptionKey, ref byte[] iv)
        {
            _symmetricEncryptionKey = symmetricEncryptionKey;
            if (iv == null) iv = GenerateIv(symmetricEncryptionKey.Length);
            _iv = iv;
        }

        /// <summary>
        /// Encrypt <paramref name="rawPlaintext"/>, using the symmetric key.
        /// <param name="rawPlaintext">The text to be encrypted.</param>
        /// <param name="iv">The IV to use for this encryption. Use null if you want a unique iv to be generated.</param>
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
            InternalContract.Require(_iv != null, "IV has not been set when calling the constructor.");
            return ConvertText(rawPlaintext, ref _iv);
        }

        /// <summary>
        /// Decrypt <paramref name="cipherText"/> using the symmetric key
        /// <param name="cipherText">The text to be decrypted</param>.
        /// <param name="iv">The IV to use for this decryption.</param>
        /// </summary>
        public byte[] Decrypt(byte[] cipherText, byte[] iv)
        {
            InternalContract.RequireNotNull(iv, nameof(iv));
            return ConvertText(cipherText, ref iv);
        }

        /// <summary>
        /// Decrypt <paramref name="cipherText"/> using the symmetric key
        /// <param name="cipherText">The text to be decrypted</param>.
        /// </summary>
        public byte[] Decrypt(byte[] cipherText)
        {
            InternalContract.Require(_iv != null, "IV has not been set when calling the constructor.");
            return ConvertText(cipherText, ref _iv);
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
                return aes.IV;
            }
        }
    }
}

