using AllOverIt.Assertion;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public sealed class RsaEncryptorFactory : IRsaEncryptorFactory
    {
        //private static readonly IRsaEncryptorFactory Instance = new RsaEncryptorFactory();

        //public IRsaEncryptor Create(string publicKeyBase64, string privateKeyBase64)
        //{
        //    _ = publicKeyBase64.WhenNotNull(nameof(publicKeyBase64));
        //    _ = privateKeyBase64.WhenNotNull(nameof(privateKeyBase64));

        //    var configuration = new RsaEncryptionConfiguration
        //    {
        //        Keys = new RsaKeyPair(publicKeyBase64, privateKeyBase64)
        //    };

        //    return new RsaEncryptor(configuration);
        //}

        //public IRsaEncryptor Create(RsaKeyPair rsaKeyPair)
        //{
        //    _ = rsaKeyPair.WhenNotNull(nameof(rsaKeyPair));

        //    var configuration = new RsaEncryptionConfiguration
        //    {
        //        Keys = rsaKeyPair
        //    };

        //    return new RsaEncryptor(configuration);
        //}

        //public IRsaEncryptor Create(RSAParameters parameters)
        //{
        //    var configuration = new RsaEncryptionConfiguration
        //    {
        //        Keys = RsaKeyPair.Create(parameters)
        //    };

        //    return new RsaEncryptor(configuration);
        //}

        public IRsaEncryptor Create(IRsaEncryptionConfiguration configuration)
        {
            _ = configuration.WhenNotNull(nameof(configuration));

            return CreateEncryptor(configuration);
        }


        public static IRsaEncryptor CreateEncryptor(string publicKeyBase64, string privateKeyBase64)
        {
            _ = publicKeyBase64.WhenNotNull(nameof(publicKeyBase64));
            _ = privateKeyBase64.WhenNotNull(nameof(privateKeyBase64));

            var configuration = new RsaEncryptionConfiguration
            {
                Keys = new RsaKeyPair(publicKeyBase64, privateKeyBase64)
            };

            return new RsaEncryptor(configuration);
        }


        public static IRsaEncryptor CreateEncryptor(RsaKeyPair rsaKeyPair)
        {
            _ = rsaKeyPair.WhenNotNull(nameof(rsaKeyPair));

            var configuration = new RsaEncryptionConfiguration
            {
                Keys = rsaKeyPair
            };

            return new RsaEncryptor(configuration);
        }

        public static IRsaEncryptor CreateEncryptor(RSAParameters parameters)
        {
            var configuration = new RsaEncryptionConfiguration
            {
                Keys = RsaKeyPair.Create(parameters)
            };

            return new RsaEncryptor(configuration);
        }

        public static IRsaEncryptor CreateEncryptor(IRsaEncryptionConfiguration configuration)
        {
            return new RsaEncryptor(configuration);
        }
    }
}
