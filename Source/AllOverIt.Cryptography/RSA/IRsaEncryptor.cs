using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public interface IRsaEncryptor : IEncryptor
    {
        IRsaEncryptionConfiguration Configuration { get; }

        // Derived from the key used for encryption (typically the public key)
        int GetMaxInputLength();

        byte[] Encrypt(byte[] plainText, RSAParameters parameters);
        byte[] Decrypt(byte[] cipherText, RSAParameters parameters);
    }
}
