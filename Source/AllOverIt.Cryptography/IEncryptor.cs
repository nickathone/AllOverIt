namespace AllOverIt.Cryptography
{
    public interface IEncryptor
    {
        byte[] Encrypt(byte[] plainText);
        byte[] Decrypt(byte[] cipherText);
    }
}
