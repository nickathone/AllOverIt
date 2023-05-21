using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes
{
    public interface IPipeConnection<TType>
    {
        public string PipeName { get; }
        public string ServerName { get; }
        public bool IsConnected { get; }

        void Connect();
        Task DisconnectAsync();
        Task WriteAsync(TType value, CancellationToken cancellationToken = default);
    }
}