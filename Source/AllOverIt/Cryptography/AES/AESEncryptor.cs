using AllOverIt.Assertion;
using System;
using System.IO;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    public sealed class AESEncryptor : IAESEncryptor
    {
        public CipherMode Mode { get; init; } = CipherMode.CBC;
        public PaddingMode Padding { get; init; } = PaddingMode.PKCS7;
        public int KeySizeBits { get; init; } = 256;
        public int BlockSize { get; init; } = 128;
        public int FeedbackSize { get; init; } = 8;
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }

        public AESEncryptor()
        {
            // Initialize the Key and IV
            using (_ = CreateAes())
            { 
            }
        }

        public AESEncryptor(byte[] key, byte[] iv)
        {
            _ = key.WhenNotNullOrEmpty(nameof(key));
            _ = iv.WhenNotNullOrEmpty(nameof(iv));

            // Will be validated at the time of encrypt / decrypt
            KeySizeBits = key.Length * 8;
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

        public byte[] Encrypt(byte[] data)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var aes = CreateAes())
                {
                    var encryptor = aes.CreateEncryptor();

                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                    }
                }

                return memoryStream.ToArray();
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            // TODO: Throw id Key / IV have not been set

            using (var memoryStream = new MemoryStream())
            {
                using (var aes = CreateAes())
                {
                    var decryptor = aes.CreateDecryptor();

                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
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
            // TODO: Throw id Key / IV have not been set

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
            aes.KeySize = KeySizeBits;      // The aes.Key will be updated if this is not the default
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
