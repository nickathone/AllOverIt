using AllOverIt.Assertion;
using System;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public sealed class RSAEncrypter : IRSAEncrypter
    {
        private readonly IRSACryptoServiceProviderFactory _cryptoServiceProviderFactory;
        private readonly RSAKeyPair _rsaKeyPair;
        private readonly bool _useOAEPPadding;      // Optimal Asymmetric Encryption Padding
        private readonly Lazy<int> _maxInputLength;
        public int MaxInputLength => _maxInputLength.Value;     // only applicable for encryption

        // While RSAKeyGenerator could be used to create a RSAKeyPair this is approach is commonly used
        public RSAEncrypter(string publicKeyBase64, string privateKeyBase64, bool useOAEPPadding = true)
            : this(new RSACryptoServiceProviderFactory(), new RSAKeyPair(publicKeyBase64, privateKeyBase64), useOAEPPadding)
        {
        }

        // RSAKeyGenerator creates RSAKeyPair from different sources such as RSAParameters, xml, or a new random pair
        // of keys with a provided key size.
        public RSAEncrypter(RSAKeyPair rsaKeyPair, bool useOAEPPadding = true)
            : this(new RSACryptoServiceProviderFactory(), rsaKeyPair, useOAEPPadding)
        {
        }

        public RSAEncrypter(RSAParameters parameters, bool useOAEPPadding = true)
            : this(RSAKeyGenerator.CreateKeyPair(parameters), useOAEPPadding)
        {
        }

        internal RSAEncrypter(IRSACryptoServiceProviderFactory cryptoServiceProviderFactory, RSAKeyPair rsaKeyPair, bool useOAEPPadding = true)
        {
            _cryptoServiceProviderFactory = cryptoServiceProviderFactory.WhenNotNull(nameof(cryptoServiceProviderFactory));
            _rsaKeyPair = rsaKeyPair.WhenNotNull(nameof(rsaKeyPair));
            _useOAEPPadding = useOAEPPadding;

            _maxInputLength = new Lazy<int>(GetMaxInputLength);
        }

        //     true to perform direct System.Security.Cryptography.RSA encryption using OAEP
        //     padding (only available on a computer running Windows XP or later); otherwise,
        //     false to use PKCS#1 v1.5 padding.
        public byte[] Encrypt(byte[] data)
        {
            _ = data.WhenNotNullOrEmpty(nameof(data));

            // TODO: Throw if _rsaKeyPair.PublicKey is null

            using (var rsa = _cryptoServiceProviderFactory.Create())
            {
                rsa.ImportRSAPublicKey(_rsaKeyPair.PublicKey, out _);

                return rsa.Encrypt(data, _useOAEPPadding);
            }
        }

        public byte[] Encrypt(byte[] data, RSAParameters parameters)
        {
            _ = data.WhenNotNullOrEmpty(nameof(data));

            using (var rsa = _cryptoServiceProviderFactory.Create())
            {
                rsa.ImportParameters(parameters);

                return rsa.Encrypt(data, _useOAEPPadding);
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            _ = data.WhenNotNullOrEmpty(nameof(data));

            using (var rsa = _cryptoServiceProviderFactory.Create())
            {
                // TODO: Throw if _rsaKeyPair.PrivateKey is null

                rsa.ImportRSAPrivateKey(_rsaKeyPair.PrivateKey, out _);

                return rsa.Decrypt(data, _useOAEPPadding);
            }
        }

        public byte[] Decrypt(byte[] data, RSAParameters parameters)
        {
            _ = data.WhenNotNullOrEmpty(nameof(data));

            using (var rsa = _cryptoServiceProviderFactory.Create())
            {
                rsa.ImportParameters(parameters);

                return rsa.Decrypt(data, _useOAEPPadding);
            }
        }

        private int GetMaxInputLength()
        {
            // TODO: Throw if _rsaKeyPair.PublicKey is null

            using (var rsa = _cryptoServiceProviderFactory.Create())
            {
                rsa.ImportRSAPublicKey(_rsaKeyPair.PublicKey, out _);

                return RSAUtils.GetMaxInputLength(rsa.KeySize, _useOAEPPadding);
            }
        }
    }
}
