using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Cryptography.RSA
{
    internal sealed class RsaFactory : IRsaFactory
    {
        public RSAAlgorithm Create()
        {
            return RSAAlgorithm.Create();
        }

        public RSAAlgorithm Create(int keySizeInBits)
        {
            return RSAAlgorithm.Create(keySizeInBits);
        }

        public RSAAlgorithm Create(RSAParameters parameters)
        {
            return RSAAlgorithm.Create(parameters);
        }
    }
}
