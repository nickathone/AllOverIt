using System.IO;

namespace AllOverIt.Cryptography
{
    public interface IStreamEncryptor
    {
        void Encrypt(Stream plainTextStream, Stream cipherTextStream);
        void Decrypt(Stream cipherTextStream, Stream plainTextStream);
    }
}
