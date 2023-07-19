namespace AllOverIt.Encryption.RSA
{
    public static class RSAUtils
    {
        public static int GetMaxInputLength(int keySizeBits, bool useOAEPPadding)
        {
            return useOAEPPadding
                ? ((keySizeBits - 384) / 8) + 7
                : ((keySizeBits - 384) / 8) + 37;
        }

        public static bool KeySizeIsValid(int keySizeBits)
        {
            return keySizeBits >= 384 &&
                   keySizeBits <= 16384 &&
                   keySizeBits % 8 == 0;
        }
    }
}
