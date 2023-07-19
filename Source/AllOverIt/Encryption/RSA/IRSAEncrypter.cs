namespace AllOverIt.Encryption.RSA
{
    public interface IRSAEncrypter
    {
        int MaxInputLength { get; }

        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] data);
    }
}
