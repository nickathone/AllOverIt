using System.Threading.Tasks;

namespace ServiceProxyDemo
{
    public interface ISecretService
    {
        string GetSecret();
        Task<string> GetSecretAsync(bool shouldThrow);
    }
}