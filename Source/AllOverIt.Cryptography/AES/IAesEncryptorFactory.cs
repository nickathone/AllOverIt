namespace AllOverIt.Cryptography.AES
{
    public interface IAesEncryptorFactory
    {
        IAesEncryptor Create();
        IAesEncryptor Create(byte[] key, byte[] iv);
    }
}
