namespace AllOverIt.Cryptography.RSA
{
    public interface IRsaEncryptorFactory
    {
        //IRsaEncryptor Create(string publicKeyBase64, string privateKeyBase64);
        //IRsaEncryptor Create(RsaKeyPair rsaKeyPair);
        //IRsaEncryptor Create(RSAParameters parameters);
        IRsaEncryptor Create(IRsaEncryptionConfiguration configuration);
    }
}
