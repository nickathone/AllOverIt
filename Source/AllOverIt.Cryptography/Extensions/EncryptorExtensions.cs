using System;
using System.IO;
using System.Text;

namespace AllOverIt.Cryptography.Extensions
{
    public static class EncryptorExtensions
    {


        public static byte[] EncryptStreamToBytes(this IStreamEncryptor encrypter, Stream plainTextStream)
        {
            using (var cipherTextStream = new MemoryStream())
            {
                encrypter.Encrypt(plainTextStream, cipherTextStream);

                return cipherTextStream.ToArray();
            }
        }

        public static string EncryptStreamToBase64(this IStreamEncryptor encrypter, Stream plainTextStream)
        {
            var bytes = EncryptStreamToBytes(encrypter, plainTextStream);

            return Convert.ToBase64String(bytes);
        }






        public static byte[] EncryptPlainTextToBytes(this IEncryptor encrypter, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);

            return encrypter.Encrypt(bytes);
        }

        public static string EncryptPlainTextToBase64(this IEncryptor encrypter, string text)
        {
            var bytes = encrypter.EncryptPlainTextToBytes(text);

            return Convert.ToBase64String(bytes);
        }

        public static string EncryptBytesToBase64(this IEncryptor encrypter, byte[] data)
        {
            var bytes = encrypter.Encrypt(data);

            return Convert.ToBase64String(bytes);
        }





        public static void DecryptBytesToStream(this IStreamEncryptor encrypter, byte[] bytes, Stream plainTextStream)
        {
            using (var cipherTextStream = new MemoryStream(bytes))
            {
                encrypter.Decrypt(cipherTextStream, plainTextStream);
            }
        }

        public static void DecryptBase64ToStream(this IStreamEncryptor encrypter, string text, Stream plainTextStream)
        {
            var bytes = Convert.FromBase64String(text);

            DecryptBytesToStream(encrypter, bytes, plainTextStream);
        }





        public static string DecryptBytesToPlainText(this IEncryptor encrypter, byte[] bytes)
        {
            var plainTextBytes = encrypter.Decrypt(bytes);

            return Encoding.UTF8.GetString(plainTextBytes);
        }

        public static string DecryptBase64ToPlainText(this IEncryptor encrypter, string text)
        {
            var bytes = Convert.FromBase64String(text);

            return encrypter.DecryptBytesToPlainText(bytes);
        }

        public static byte[] DecryptBase64ToBytes(this IEncryptor encrypter, string text)
        {
            var bytes = Convert.FromBase64String(text);

            return encrypter.Decrypt(bytes);
        }
    }
}
