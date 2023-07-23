using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    public interface IAesEncryptor : IEncryptor, IStreamEncryptor
    {
        CipherMode Mode { get; set; }
        PaddingMode Padding { get; set; }
        int KeySize { get; set; }               // Bytes
        int BlockSize { get; set; }
        int FeedbackSize { get; set; }
        byte[] Key { get; }
        byte[] IV { get; }

        void ResetKey();
        void ResetIV();

#if !NETSTANDARD2_1
        int GetCipherTextLength(int plainTextLength);
#endif        
    }
}
