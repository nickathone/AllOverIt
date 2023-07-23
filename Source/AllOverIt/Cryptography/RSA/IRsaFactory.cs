using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Cryptography.RSA
{
    public interface IRsaFactory
    {
        RSAAlgorithm Create();
        RSAAlgorithm Create(int keySizeInBits);
        RSAAlgorithm Create(RSAParameters parameters);
    }
}
