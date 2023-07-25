using AllOverIt.Cryptography.RSA;

namespace AllOverIt.Cryptography.Hybrid
{
    public sealed class RsaAesHybridEncryptorConfiguration : IRsaAesHybridEncryptorConfiguration
    {
        public IRsaEncryptionConfiguration Encryption { get; init; } = new RsaEncryptionConfiguration();
        public IRsaSigningConfiguration Signing { get; init; } = new RsaSigningConfiguration();
    }
}
