using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public interface IRSACryptoServiceProviderFactory
    {
        RSACryptoServiceProvider Create();
    }
}
