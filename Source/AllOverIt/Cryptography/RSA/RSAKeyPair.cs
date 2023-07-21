using AllOverIt.Extensions;
using System;

namespace AllOverIt.Cryptography.RSA
{
    public sealed class RSAKeyPair
    {
        // these can be null (may only want to encrypt or decrypt)
        public byte[] PublicKey { get; init; }
        public byte[] PrivateKey { get; init; }

        public RSAKeyPair()
        {
        }

        public RSAKeyPair(System.Security.Cryptography.RSA rsa)
        {
            PublicKey = rsa.ExportRSAPublicKey();
            PrivateKey = rsa.ExportRSAPrivateKey();
        }

        // these values can be null / empty
        public RSAKeyPair(string publicKeyBase64, string privateKeyBase64)
        {
            PublicKey = GetAsBytes(publicKeyBase64);
            PrivateKey = GetAsBytes(privateKeyBase64);
        }

        private static byte[] GetAsBytes(string key)
        {
            if (key.IsNullOrEmpty())
            {
                return null;
            }

            return Convert.FromBase64String(key);
        }
    }
}
