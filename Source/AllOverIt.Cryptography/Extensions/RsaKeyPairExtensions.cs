using AllOverIt.Cryptography.RSA;
using System;

namespace AllOverIt.Cryptography.Extensions
{
    public static class RsaKeyPairExtensions
    {
        public static string GetPublicKeyAsBase64(this RsaKeyPair rsaKeyPair)
        {
            return Convert.ToBase64String(rsaKeyPair.PublicKey);
        }

        public static string GetPrivateKeyAsBase64(this RsaKeyPair rsaKeyPair)
        {
            return Convert.ToBase64String(rsaKeyPair.PrivateKey); ;
        }
    }
}
