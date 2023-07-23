using System.IO;

namespace AllOverIt.Cryptography
{
    public interface IStreamEncryptor
    {
        void Encrypt(Stream source, Stream destination);
        void Decrypt(Stream source, Stream destination);
    }
}
