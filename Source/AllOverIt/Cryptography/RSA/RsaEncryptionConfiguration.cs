using AllOverIt.Assertion;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public sealed class RsaEncryptionConfiguration : IRsaEncryptionConfiguration
    {
        public RsaKeyPair Keys { get; init; } = RsaKeyPair.Create();
        public RSAEncryptionPadding Padding { get; init; } = RSAEncryptionPadding.OaepSHA256;

        public RsaEncryptionConfiguration()
        {
        }

        public RsaEncryptionConfiguration(RsaKeyPair keys)
        {
            Keys = keys.WhenNotNull(nameof(keys));
        }
    }
}
