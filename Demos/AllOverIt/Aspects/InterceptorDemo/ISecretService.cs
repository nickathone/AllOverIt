using System.Threading.Tasks;

namespace InterceptorDemo
{
    public interface ISecretService
    {
        string GetSecret();
        Task<string> GetSecretAsync(bool shouldThrow);
    }
}