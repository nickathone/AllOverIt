using AllOverIt.Assertion;
using System;
using System.IO;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    public sealed class AesEncryptor : IAesEncryptor
    {
        // TODO: Add a IAesConfiguration (remove from IAesEncryptor)
        public CipherMode Mode { get; set; } = CipherMode.CBC;
        public PaddingMode Padding { get; set; } = PaddingMode.PKCS7;
        
        // In bits
        public int KeySize { get; set; } = 256;

        public int BlockSize { get; set; } = 128;
        public int FeedbackSize { get; set; } = 8;
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }

        public AesEncryptor()
        {
            // Initialize the Key and IV
            using (_ = CreateAes())
            { 
            }
        }

        public AesEncryptor(byte[] key, byte[] iv)
        {
            _ = key.WhenNotNullOrEmpty(nameof(key));
            _ = iv.WhenNotNullOrEmpty(nameof(iv));

            // Will be validated at the time of encrypt / decrypt
            KeySize = key.Length * 8;
            Key = key;
            IV = iv;
        }

        public void ResetKey()
        {
            Key = null;
        }

        public void ResetIV()
        {
            IV = null;
        }

#if !NETSTANDARD2_1
        public int GetCipherTextLength(int plainTextLength)
        {
            using (var aes = CreateAes())
            {
                return Mode switch
                {
                    CipherMode.CBC => aes.GetCiphertextLengthCbc(plainTextLength, Padding),
                    CipherMode.CFB => aes.GetCiphertextLengthCfb(plainTextLength, Padding),
                    CipherMode.ECB => aes.GetCiphertextLengthEcb(plainTextLength, Padding),

                    // TODO: custom exception
                    _ => throw new InvalidOperationException($"The {Mode} cipher mode is not valid for the AES algorithm."),
                };
            }
        }
#endif

        public byte[] Encrypt(byte[] plainText)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var aes = CreateAes())
                {
                    var encryptor = aes.CreateEncryptor();

                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainText, 0, plainText.Length);
                    }
                }

                return memoryStream.ToArray();
            }
        }

        public byte[] Decrypt(byte[] cipherText)
        {
            // TODO: Throw id Key / IV have not been set

            using (var memoryStream = new MemoryStream())
            {
                using (var aes = CreateAes())
                {
                    var decryptor = aes.CreateDecryptor();

                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(cipherText, 0, cipherText.Length);
                    }
                }

                return memoryStream.ToArray();
            }
        }

        public void Encrypt(Stream source, Stream destination)
        {
            using (var aes = CreateAes())
            {
                var encryptor = aes.CreateEncryptor();

                using (var cryptoStream = new CryptoStream(destination, encryptor, CryptoStreamMode.Write))
                {
                    source.CopyTo(cryptoStream);
                }
            }
        }

        public void Decrypt(Stream source, Stream destination)
        {
            // TODO: Throw if Key / IV have not been set

            using (var aes = CreateAes())
            {
                var decryptor = aes.CreateDecryptor();

                using (var cryptoStream = new CryptoStream(destination, decryptor, CryptoStreamMode.Write))
                {
                    source.CopyTo(cryptoStream);
                }
            }
        }

        private Aes CreateAes()
        {
            var aes = Aes.Create();

            aes.Mode = Mode;
            aes.Padding = Padding;
            aes.KeySize = KeySize;          // The aes.Key will be updated if this is not the default
            aes.BlockSize = BlockSize;
            aes.FeedbackSize = FeedbackSize;

            // Key / IV will be initialized during the first Encrypt() if they have not been previously set
            Key ??= aes.Key;
            IV ??= aes.IV;

            aes.Key = Key;
            aes.IV = IV;

            // TODO: Custom exceptions
            Throw<InvalidOperationException>.When(Key.Length != aes.KeySize / 8, $"The AES Key must be {aes.KeySize / 8} bytes.");
            Throw<InvalidOperationException>.When(IV.Length != 16, "The AES Initialization Vector must be 16 bytes.");

            return aes;
        }
    }
}
