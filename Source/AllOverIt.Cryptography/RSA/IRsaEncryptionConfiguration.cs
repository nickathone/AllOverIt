using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public interface IRsaEncryptionConfiguration
    {
        RsaKeyPair Keys { get; }
        RSAEncryptionPadding Padding { get; }
    }
}
