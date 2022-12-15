using System;
using System.Threading.Tasks;

namespace ProxyDecoratorDemo
{
    internal sealed class SecretService : ISecretService
    {
        public string GetSecret()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<string> GetSecretAsync(bool shouldThrow)
        {
            await Task.Delay(1000);

            if (shouldThrow)
            {
                throw new Exception("Test Exception");
            }

            return GetSecret();
        }
    }
}