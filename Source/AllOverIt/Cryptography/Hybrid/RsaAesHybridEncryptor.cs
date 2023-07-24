using AllOverIt.Assertion;
using AllOverIt.Cryptography.AES;
using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.RSA;
using System;
using System.IO;

namespace AllOverIt.Cryptography.Hybrid
{
    public sealed class RsaAesHybridEncryptor : IRsaAesHybridEncryptor
    {
        private readonly IRsaFactory _rsaFactory;
        private readonly IRsaEncryptor _rsaEncryptor;
        private readonly IAesEncryptorFactory _aesEncryptorFactory;


        private readonly IRsaSigningConfiguration _signingConfiguration;
        
        public RsaAesHybridEncryptor(IRsaAesHybridEncryptorConfiguration configuration)
            : this(
                  new RsaFactory(),
                  RsaEncryptor.Create(configuration.Encryption),
                  new AesEncryptorFactory(),
                  configuration.Signing)
        {
        }

        internal RsaAesHybridEncryptor(IRsaFactory rsaFactory, IRsaEncryptor rsaEncryptor,
            IAesEncryptorFactory aesEncryptorFactory, IRsaSigningConfiguration signingConfiguration)
        {
            _rsaFactory = rsaFactory.WhenNotNull(nameof(rsaFactory));
            _rsaEncryptor = rsaEncryptor.WhenNotNull(nameof(rsaEncryptor));
            _aesEncryptorFactory = aesEncryptorFactory.WhenNotNull(nameof(aesEncryptorFactory));
            _signingConfiguration = signingConfiguration.WhenNotNull(nameof(signingConfiguration));
        }

        public byte[] Encrypt(byte[] plainText)
        {
            // TODO: Throw if there's no RSA private key
            // TODO: Add an option to choose public/private or private/public keys for encryption/decryption

            using (var memoryStream = new MemoryStream())
            {
                // Calculate the hash for the plain text
                var hash = CalculateHash(plainText);

                // Sign the plain text hash
                var signature = SignHash(hash);

                // Prepare AES encryptor with a random Key and IV
                var aesEncryptor = _aesEncryptorFactory.Create();

                // RSA encrypt the AES key
                var rsaEncryptedAesKey = _rsaEncryptor.Encrypt(aesEncryptor.Key);

                // AES encrypt the plain text
                var cipherText = aesEncryptor.Encrypt(plainText);

                // Write the data to the stream
                memoryStream.Write(signature);              // A known length (same as RSA key size, 3072 bits = 384 bytes)
                memoryStream.Write(hash);                   // A known length (based on the signature hash algorithm, SHA256 = 256 bits = 32 bytes)
                memoryStream.Write(aesEncryptor.IV);        // A known length (always 16 bytes)
                memoryStream.Write(rsaEncryptedAesKey);     // Can be calculated using AesUtils.GetCipherTextLength()
                memoryStream.Write(cipherText);             // Can be calculated using AesUtils.GetCipherTextLength() (remainder of the stream)

                //var encryptedAesKeyLength = AesUtils.GetCipherTextLength(rsaEncryptedAesKey.Length);
                //var cipherTextLength = AesUtils.GetCipherTextLength(plainText.Length);

                return memoryStream.ToArray();
            }
        }

        public byte[] Decrypt(byte[] cipherText)
        {
            using (var memoryStream = new MemoryStream(cipherText))
            {
                // Read the signature
                     // In bytes
                var signature = ReadFromStream(memoryStream, _rsaEncryptor.Configuration.Keys.KeySize / 8);

                // Read the expected hash of the plain text
                var hashLength = _signingConfiguration.HashAlgorithmName.GetHashSize() / 8;
                var hash = new byte[hashLength];
                memoryStream.Read(hash);

                // Read the AES IV
                var iv = new byte[16];
                memoryStream.Read(iv);

                // Determine the AES key
                var rsaEncryptedAesKeyLength = _rsaEncryptor.Configuration.Keys.KeySize / 8;
                var rsaEncryptedAesKey = new byte[rsaEncryptedAesKeyLength];
                memoryStream.Read(rsaEncryptedAesKey);
                var aesKey = _rsaEncryptor.Decrypt(rsaEncryptedAesKey);

                // Read the cipher text
                var remaining = memoryStream.Length - memoryStream.Position;
                var encryptedPlainText = new byte[remaining];
                memoryStream.Read(encryptedPlainText);

                // Decrypt the cipher text (in the stream)
                var aesEncryptor = _aesEncryptorFactory.Create(aesKey, iv);
                var plainText = aesEncryptor.Decrypt(encryptedPlainText);

                // Calculate the hash of the plain text
                var plainTextHash = CalculateHash(plainText);

                // Verify the signature
                using (var rsa = _rsaFactory.Create())
                {
                    rsa.ImportRSAPublicKey(_rsaEncryptor.Configuration.Keys.PublicKey, out _);

                    var isValid = rsa.VerifyHash(plainTextHash, signature, _signingConfiguration.HashAlgorithmName, _signingConfiguration.Padding);

                    // TODO: Custom exception
                    Throw<InvalidOperationException>.WhenNot(isValid, "The digital signature is invalid.");
                }

                return plainText;
            }
        }

        public void Encrypt(Stream source, Stream destination)
        {

        }

        public void Decrypt(Stream source, Stream destination)
        {

        }



        private byte[] CalculateHash(byte[] plainText)
        {
            using (var hashAlgorithm = _signingConfiguration.HashAlgorithmName.CreateHashAlgorithm())
            {
                return hashAlgorithm.ComputeHash(plainText);
            }
        }

        private byte[] SignHash(byte[] hash)
        {
            using (var rsa = _rsaFactory.Create())
            {
                var rsaPrivateKey = _rsaEncryptor.Configuration.Keys.PrivateKey;

                rsa.ImportRSAPrivateKey(rsaPrivateKey, out _);

                return rsa.SignHash(hash, _signingConfiguration.HashAlgorithmName, _signingConfiguration.Padding);
            }
        }

        private static byte[] ReadFromStream(Stream stream, int length)
        {
            var data = new byte[length];
            stream.Read(data);

            return data;
        }
    }
}
