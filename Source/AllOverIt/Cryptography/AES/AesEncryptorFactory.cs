using AllOverIt.Assertion;

namespace AllOverIt.Cryptography.AES
{
    public sealed class AesEncryptorFactory : IAesEncryptorFactory
    {
        public IAesEncryptor Create()
        {
            return new AesEncryptor();
        }

        public IAesEncryptor Create(byte[] key, byte[] iv)
        {
            _ = key.WhenNotNullOrEmpty(nameof(key));
            _ = iv.WhenNotNullOrEmpty(nameof(iv));

            return new AesEncryptor(key, iv);
        }
    }
}
