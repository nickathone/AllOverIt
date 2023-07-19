using System.Security.Cryptography;

namespace AllOverIt.Encryption.RSA
{
    internal sealed class RSACryptoServiceProviderFactory : IRSACryptoServiceProviderFactory
    {
        public RSACryptoServiceProvider Create()
        {
            return new RSACryptoServiceProvider();
        }
    }
}
