using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public interface IRSAEncryptor : IEncryptor
    {
        int MaxInputLength { get; }

        byte[] Encrypt(byte[] data, RSAParameters parameters);
        byte[] Decrypt(byte[] data, RSAParameters parameters);
    }
}
