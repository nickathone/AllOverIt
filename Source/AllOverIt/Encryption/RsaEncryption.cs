using AllOverIt.Assertion;
using System.Security.Cryptography;
using System.Text;

namespace AllOverIt.Encryption
{


    public sealed class RSAEncrypter
    {
        private readonly IRSAFactory _rsaFactory;

        public RSAEncrypter()
            : this(new RSAFactory())
        {
        }

        internal RSAEncrypter(IRSAFactory rsaFactory)
        {
            _rsaFactory = rsaFactory.WhenNotNull(nameof(rsaFactory));
        }

        //public byte[] Encrypt(string data, RSAParameters parameters, bool useOAEPPadding = true)
        //{
        //    _ = data.WhenNotNullOrEmpty(nameof(data));

        //    var bytes = Encoding.UTF8.GetBytes(data);

        //    return Encrypt(bytes, parameters, useOAEPPadding);
        //}

        //public byte[] Encrypt(byte[] data, RSAParameters parameters, bool useOAEPPadding = true)
        //{
        //    _ = data.WhenNotNullOrEmpty(nameof(data));

        //    using (var rsa = _rsaFactory.CreateFromRSAParameters(parameters))
        //    {
        //        rsa.ImportParameters(parameters);
        //        //rsa.ExportRSAPrivateKey

        //        return rsa.Encrypt(data, useOAEPPadding);
        //    }
        //}

    }



    public interface IRSAFactory
    {
        RSA CreateFromRSAParameters(RSAParameters rsaParameters);
        RSA CreateFromXml(string xml);
        RSA CreateWithKeySize(int keySizeBits);
    }



    public sealed class RSAFactory : IRSAFactory
    {
        public RSA CreateWithKeySize(int keySizeBits)
        {
            return RSA.Create(keySizeBits);
        }

        public RSA CreateFromRSAParameters(RSAParameters rsaParameters)
        {
            return RSA.Create(rsaParameters);
        }

        public RSA CreateFromXml(string xml)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xml);

            return rsa;
        }
    }

}
