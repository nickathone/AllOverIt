using System.Security.Cryptography;

namespace AllOverIt.Encryption.RSA
{
    public interface IRSACryptoServiceProviderFactory
    {
        RSACryptoServiceProvider Create();
    }
}
