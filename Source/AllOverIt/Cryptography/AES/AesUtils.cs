using System;

namespace AllOverIt.Cryptography.AES
{
    public static class AesUtils
    {
        public static int GetCipherTextLength(int plainTextLength)
        {
            var numBlocks = (int)Math.Ceiling(plainTextLength / 16.0d);

            return numBlocks * 16;
        }
    }
}
