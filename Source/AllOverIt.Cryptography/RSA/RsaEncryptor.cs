using AllOverIt.Assertion;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public sealed class RsaEncryptor : IRsaEncryptor
    {
        private readonly IRsaFactory _rsaFactory;
        
        private int? _maxInputLength;

        public IRsaEncryptionConfiguration Configuration { get; }

        public RsaEncryptor(IRsaEncryptionConfiguration configuration)
            : this(new RsaFactory(), configuration)
        {
        }

        internal RsaEncryptor(IRsaFactory rsaFactory, IRsaEncryptionConfiguration configuration)
        {
            _rsaFactory = rsaFactory.WhenNotNull(nameof(rsaFactory));
            Configuration = configuration.WhenNotNull(nameof(configuration));
        }

        public int GetMaxInputLength()
        {
            // TODO: Throw if _rsaKeyPair.PublicKey is null

            if (!_maxInputLength.HasValue)
            {
                using (var rsa = _rsaFactory.Create())
                {
                    rsa.ImportRSAPublicKey(Configuration.Keys.PublicKey, out _);

                    _maxInputLength = RsaUtils.GetMaxInputLength(rsa.KeySize, Configuration.Padding);
                }
            }

            return _maxInputLength.Value;
        }

        public byte[] Encrypt(byte[] plainText)
        {
            _ = plainText.WhenNotNullOrEmpty(nameof(plainText));

            // TODO: Throw if _rsaKeyPair.PublicKey is null

            using (var rsa = _rsaFactory.Create())
            {
                rsa.ImportRSAPublicKey(Configuration.Keys.PublicKey, out _);

                return rsa.Encrypt(plainText, Configuration.Padding);
            }
        }

        public byte[] Encrypt(byte[] plainText, RSAParameters parameters)
        {
            _ = plainText.WhenNotNullOrEmpty(nameof(plainText));

            using (var rsa = _rsaFactory.Create())
            {
                rsa.ImportParameters(parameters);

                return rsa.Encrypt(plainText, Configuration.Padding);
            }
        }

        public byte[] Decrypt(byte[] cipherText)
        {
            _ = cipherText.WhenNotNullOrEmpty(nameof(cipherText));

            using (var rsa = _rsaFactory.Create())
            {
                // TODO: Throw if _rsaKeyPair.PrivateKey is null

                rsa.ImportRSAPrivateKey(Configuration.Keys.PrivateKey, out _);

                return rsa.Decrypt(cipherText, Configuration.Padding);
            }
        }

        public byte[] Decrypt(byte[] cipherText, RSAParameters parameters)
        {
            _ = cipherText.WhenNotNullOrEmpty(nameof(cipherText));

            using (var rsa = _rsaFactory.Create())
            {
                rsa.ImportParameters(parameters);

                return rsa.Decrypt(cipherText, Configuration.Padding);
            }
        }

        public static IRsaEncryptor Create(string publicKeyBase64, string privateKeyBase64)
        {
            _ = publicKeyBase64.WhenNotNull(nameof(publicKeyBase64));
            _ = privateKeyBase64.WhenNotNull(nameof(privateKeyBase64));

            var configuration = new RsaEncryptionConfiguration
            {
                Keys = new RsaKeyPair(publicKeyBase64, privateKeyBase64)
            };

            return new RsaEncryptor(configuration);
        }

        public static IRsaEncryptor Create(RsaKeyPair rsaKeyPair)
        {
            _ = rsaKeyPair.WhenNotNull(nameof(rsaKeyPair));

            var configuration = new RsaEncryptionConfiguration
            {
                Keys = rsaKeyPair
            };

            return new RsaEncryptor(configuration);
        }

        public static IRsaEncryptor Create(RSAParameters parameters)
        {
            var configuration = new RsaEncryptionConfiguration
            {
                Keys = RsaKeyPair.Create(parameters)
            };

            return new RsaEncryptor(configuration);
        }

        public static IRsaEncryptor Create(IRsaEncryptionConfiguration configuration)
        {
            return new RsaEncryptor(configuration);
        }
    }
}
