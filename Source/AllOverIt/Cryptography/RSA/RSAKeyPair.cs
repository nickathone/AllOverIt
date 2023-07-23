using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Cryptography.RSA
{
    public sealed class RsaKeyPair
    {
        public byte[] PublicKey { get; init; }
        public byte[] PrivateKey { get; init; }
        public int KeySize { get; private set; }            // in bytes

        public RsaKeyPair(RSAAlgorithm rsa)
        {
            PublicKey = rsa.ExportRSAPublicKey();
            PrivateKey = rsa.ExportRSAPrivateKey();
            KeySize = rsa.KeySize / 8;
        }

        // these values can be null / empty
        public RsaKeyPair(string publicKeyBase64, string privateKeyBase64)
        {
            // TODO: Custom exception
            Throw<InvalidOperationException>.When(
                publicKeyBase64.IsNullOrEmpty() && privateKeyBase64.IsNullOrEmpty(),
                "At least one RSA key is required.");

            PublicKey = GetAsBytes(publicKeyBase64);
            PrivateKey = GetAsBytes(privateKeyBase64);

            using (var rsa = RSAAlgorithm.Create())
            {
                if (PublicKey.Length != 0)
                {
                    rsa.ImportRSAPublicKey(PublicKey, out _);
                }
                else
                {
                    rsa.ImportRSAPrivateKey(PrivateKey, out _);
                }

                KeySize = rsa.KeySize / 8;
            }
        }

        public static RsaKeyPair Create(int keySizeInBits = 3072)
        {
            using (var rsa = RSAAlgorithm.Create(keySizeInBits))
            {
                return new RsaKeyPair(rsa);
            }
        }

        public static RsaKeyPair Create(RSAParameters parameters)
        {
            using (var rsa = RSAAlgorithm.Create(parameters))
            {
                return new RsaKeyPair(rsa);
            }
        }

        public static RsaKeyPair Create(string xmlKeys)
        {
            using (var rsa = RSAAlgorithm.Create())
            {
                rsa.FromXmlString(xmlKeys);

                return new RsaKeyPair(rsa);
            }
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
