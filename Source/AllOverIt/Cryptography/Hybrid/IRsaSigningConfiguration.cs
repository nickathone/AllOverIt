using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Hybrid
{
    public interface IRsaSigningConfiguration
    {
        HashAlgorithmName HashAlgorithmName { get; }
        RSASignaturePadding Padding { get; }
    }
}
