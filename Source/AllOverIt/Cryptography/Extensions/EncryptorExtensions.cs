using System;
using System.Text;

namespace AllOverIt.Cryptography.Extensions
{
    public static class EncryptorExtensions
    {
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

        public static string DecryptBytesToPlainText(this IEncryptor encrypter, byte[] bytes)
        {
            var data = encrypter.Decrypt(bytes);

            return Encoding.UTF8.GetString(data);
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
