using AllOverIt.Cryptography.RSA;

namespace AllOverIt.Cryptography.Hybrid
{
    public interface IRsaAesHybridEncryptorConfiguration
    {
        public IRsaEncryptionConfiguration Encryption { get; }
        IRsaSigningConfiguration Signing { get; }
    }
}
