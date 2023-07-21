namespace AllOverIt.Cryptography.RSA
{
    public static class RSAUtils
    {
        public static int GetMaxInputLength(int keySizeBits, bool useOAEPPadding)
        {
            var keySizeBytes = keySizeBits / 8;

            return useOAEPPadding
                ? keySizeBytes - 42
                : keySizeBytes;
        }

        public static bool KeySizeIsValid(int keySizeBits)
        {
            return keySizeBits >= 384 &&
                   keySizeBits <= 16384 &&
                   keySizeBits % 8 == 0;
        }
    }
}
