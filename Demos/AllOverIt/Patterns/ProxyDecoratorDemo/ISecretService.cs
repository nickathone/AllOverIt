using System.Threading.Tasks;

namespace ProxyDecoratorDemo
{
    public interface ISecretService
    {
        string GetSecret();
        Task<string> GetSecretAsync(bool shouldThrow);
    }
}