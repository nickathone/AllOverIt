using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Extensions
{
    public static class HashAlgorithmNameExtensions
    {
        public static HashAlgorithm CreateHashAlgorithm(this HashAlgorithmName algorithmName)
        {
            var registry = new Dictionary<HashAlgorithmName, Func<HashAlgorithm>>
            {
                { HashAlgorithmName.MD5, () => MD5.Create() },
                { HashAlgorithmName.SHA1, () => SHA1.Create() },
                { HashAlgorithmName.SHA256, () => SHA256.Create() },
                { HashAlgorithmName.SHA384 ,() => SHA384.Create() },
                { HashAlgorithmName.SHA512,() => SHA512.Create() }
            };

            if (registry.TryGetValue(algorithmName, out var factory))
            {
                return factory.Invoke();
            }

            // TODO: Custom exception
            throw new InvalidOperationException($"Unknown hash algorithm {algorithmName.Name}.");
        }

        // In bits
        public static int GetHashSize(this HashAlgorithmName algorithmName)
        {
            var registry = new Dictionary<HashAlgorithmName, int>
            {
                { HashAlgorithmName.MD5, 128 },
                { HashAlgorithmName.SHA1, 160 },
                { HashAlgorithmName.SHA256, 256 },
                { HashAlgorithmName.SHA384, 384 },
                { HashAlgorithmName.SHA512, 512 }
            };

            if (registry.TryGetValue(algorithmName, out var sizeInBits))
            {
                return sizeInBits;
            }

            // TODO: Custom exception
            throw new InvalidOperationException($"Unknown hash algorithm {algorithmName.Name}.");
        }
    }
}
