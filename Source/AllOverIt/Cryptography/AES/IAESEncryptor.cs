using System.IO;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    public interface IAESEncryptor : IEncryptor
    {
        CipherMode Mode { get; }
        PaddingMode Padding { get; }
        int KeySizeBits { get; }
        int BlockSize { get; }
        int FeedbackSize { get; }
        byte[] Key { get; }
        byte[] IV { get; }

        void ResetKey();
        void ResetIV();

#if !NETSTANDARD2_1
        int GetCipherTextLength(int plainTextLength);
#endif

        void Encrypt(Stream source, Stream destination);
        void Decrypt(Stream source, Stream destination);
    }
}
