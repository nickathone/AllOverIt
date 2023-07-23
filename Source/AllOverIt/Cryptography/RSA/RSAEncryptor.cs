using AllOverIt.Assertion;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public sealed class RsaEncryptor : IRsaEncryptor
    {
        private readonly IRsaFactory _rsaFactory;
        
        private int? _maxInputLength;

        public IRsaEncryptionConfiguration Configuration { get; }

        // While RSAKeyGenerator could be used to create a RSAKeyPair this is approach is commonly used
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

        //     true to perform direct System.Security.Cryptography.RSA encryption using OAEP
        //     padding (only available on a computer running Windows XP or later); otherwise,
        //     false to use PKCS#1 v1.5 padding.
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
    }
}
