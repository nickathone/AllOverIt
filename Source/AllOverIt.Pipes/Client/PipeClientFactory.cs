using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.Pipes.Connection;

namespace AllOverIt.Pipes.Client
{
    internal static class PipeClientFactory
    {
        public static async Task<PipeReaderWriter> ConnectAsync(string pipeName, string serverName, CancellationToken cancellationToken = default)
        {
            var pipeStream = await CreateAndConnectAsync(pipeName, serverName, cancellationToken).ConfigureAwait(false);

            return new PipeReaderWriter(pipeStream);
        }

        public static async Task<NamedPipeClientStream> CreateAndConnectAsync(string pipeName, string serverName, CancellationToken cancellationToken = default)
        {
            var pipe = Create(pipeName, serverName);

            try
            {
                await pipe
                    .ConnectAsync(cancellationToken)
                    .ConfigureAwait(false);

                return pipe;
            }
            catch
            {
                await pipe
                    .DisposeAsync()
                    .ConfigureAwait(false);

                throw;
            }
        }

        public static NamedPipeClientStream Create(string pipeName, string serverName)
        {
            return new NamedPipeClientStream(
                serverName,
                pipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }
    }
}