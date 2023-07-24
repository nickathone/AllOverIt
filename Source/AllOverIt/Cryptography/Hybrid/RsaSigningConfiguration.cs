using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Hybrid
{
    public sealed class RsaSigningConfiguration : IRsaSigningConfiguration
    {
        public HashAlgorithmName HashAlgorithmName { get; init; } = HashAlgorithmName.SHA256;
        public RSASignaturePadding Padding { get; init; } = RSASignaturePadding.Pkcs1;
    }
}
