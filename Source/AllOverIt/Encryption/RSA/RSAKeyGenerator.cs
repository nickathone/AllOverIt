using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Encryption.RSA
{
    public static class RSAKeyGenerator
    {
        public static RSAKeyPair CreateKeyPair(int keySizeBits = 3072)
        {
            // Useful reading:
            // https://www.scottbrady91.com/c-sharp/rsa-key-loading-dotnet
            // https://www.scottbrady91.com/jose/jwts-which-signing-algorithm-should-i-use
            using (var rsa = RSAAlgorithm.Create(keySizeBits))
            {
                return new RSAKeyPair(rsa);
            }
        }

        public static RSAKeyPair CreateKeyPair(RSAParameters parameters)
        {
            using (var rsa = RSAAlgorithm.Create(parameters))
            {
                return new RSAKeyPair(rsa);
            }
        }

        public static RSAKeyPair CreateKeyPair(string xmlKeys)
        {
            using (var rsa = RSAAlgorithm.Create())
            {
                rsa.FromXmlString(xmlKeys);
                return new RSAKeyPair(rsa);
            }
        }
    }
}
