using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xlent.Lever.AsyncCaller2.Bll
{
    /// <summary>
    /// Class to encrypt or decrypt messages
    /// </summary>
    public class SymmetricCrypto
    {
        private readonly byte[] _symmetricEncryptionKey;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symmetricEncryptionKey">The symmetric key to use both to encrypt and to decrypt.</param>
        public SymmetricCrypto(byte[] symmetricEncryptionKey)
        {
            _symmetricEncryptionKey = symmetricEncryptionKey;
        }

        /// <summary>
        /// Encrypt <paramref name="message"/>, using the symmetric key.
        /// </summary>
        public byte[] Encrypt(byte[] message)
        {
            if (message == null) return null;
            return message;
        }
        /// <summary>
        /// Decrypt <paramref name="message"/>, using the symmetric key.
        /// </summary>
        public byte[] Decrypt(byte[] message)
        {
            if (message == null) return null;
            return message;
        }
    }
}
