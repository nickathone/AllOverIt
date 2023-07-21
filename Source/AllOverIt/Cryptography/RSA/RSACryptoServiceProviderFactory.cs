using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    internal sealed class RSACryptoServiceProviderFactory : IRSACryptoServiceProviderFactory
    {
        public RSACryptoServiceProvider Create()
        {
            return new RSACryptoServiceProvider();
        }
    }
}
