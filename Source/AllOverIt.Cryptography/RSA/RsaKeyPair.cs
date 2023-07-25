using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Cryptography.RSA
{
    public sealed class RsaKeyPair
    {
        public byte[] PublicKey { get; private set; }
        public byte[] PrivateKey { get; private set; }

        /// <summary>Gets the size, in bits, of the key modulus use by the RSA algorithm.</summary>
        public int KeySize { get; private set; }

        public RsaKeyPair(RSAAlgorithm rsa)
        {
            var publicKey = rsa.ExportRSAPublicKey();
            var privateKey = rsa.ExportRSAPrivateKey();

            SetKeys(publicKey, privateKey);
        }

        // these values can be null / empty
        public RsaKeyPair(string publicKeyBase64, string privateKeyBase64)
        {
            SetKeys(publicKeyBase64, privateKeyBase64);
        }

        public void SetKeys(string publicKeyBase64, string privateKeyBase64)
        {
            // TODO: Custom exception
            Throw<InvalidOperationException>.When(
                publicKeyBase64.IsNullOrEmpty() && privateKeyBase64.IsNullOrEmpty(),
                "At least one RSA key is required.");

            var publicKey = GetAsBytes(publicKeyBase64);
            var privateKey = GetAsBytes(privateKeyBase64);

            SetKeys(publicKey, privateKey);
        }

        public void SetKeys(byte[] publicKeyBase64, byte[] privateKeyBase64)
        {
            // TODO: Custom exception
            Throw<InvalidOperationException>.When(
                publicKeyBase64.IsNullOrEmpty() && privateKeyBase64.IsNullOrEmpty(),
                "At least one RSA key is required.");

            PublicKey = publicKeyBase64;
            PrivateKey = privateKeyBase64;
            KeySize = GetKeySize();
        }

        public static RsaKeyPair Create(RSAAlgorithm rsa)
        {
            return new RsaKeyPair(rsa);
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

        private int GetKeySize()
        {
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

                return rsa.KeySize;
            }
        }
    }
}
