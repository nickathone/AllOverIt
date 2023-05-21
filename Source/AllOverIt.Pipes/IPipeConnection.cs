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

        // Gets the username of the connected client.  Note that we will not have access to the client's
        // username until it has written at least once to the pipe (and has set its impersonationLevel
        // argument appropriately).
        string GetImpersonationUserName();
    }
}