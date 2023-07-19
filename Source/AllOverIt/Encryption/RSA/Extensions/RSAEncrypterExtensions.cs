using System;
using System.Text;

namespace AllOverIt.Encryption.RSA.Extensions
{
    public static class RSAEncrypterExtensions
    {
        public static byte[] EncryptTextToBytes(this IRSAEncrypter encrypter, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);

            return encrypter.Encrypt(bytes);
        }

        public static string EncryptTextToBase64(this IRSAEncrypter encrypter, string text)
        {
            var bytes = encrypter.EncryptTextToBytes(text);

            return Convert.ToBase64String(bytes);
        }

        public static string EncryptBytesToBase64(this IRSAEncrypter encrypter, byte[] data)
        {
            var bytes = encrypter.Encrypt(data);

            return Convert.ToBase64String(bytes);
        }

        public static string DecryptBytesToText(this IRSAEncrypter encrypter, byte[] bytes)
        {
            var data = encrypter.Decrypt(bytes);

            return Encoding.UTF8.GetString(data);
        }

        public static string DecryptBase64ToText(this IRSAEncrypter encrypter, string text)
        {
            var bytes = Convert.FromBase64String(text);

            return encrypter.DecryptBytesToText(bytes);
        }

        public static byte[] DecryptBase64ToBytes(this IRSAEncrypter encrypter, string text)
        {
            var bytes = Convert.FromBase64String(text);

            return encrypter.Decrypt(bytes);
        }
    }
}
