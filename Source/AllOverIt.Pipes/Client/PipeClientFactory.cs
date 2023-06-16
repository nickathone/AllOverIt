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

            return new PipeReaderWriter(pipeStream, false);
        }

        public static async Task<NamedPipeClientStream> CreateAndConnectAsync(string pipeName, string serverName, CancellationToken cancellationToken = default)
        {
            var pipeStream = CreateNamedPipeClientStream(pipeName, serverName);

            try
            {
                await pipeStream
                    .ConnectAsync(cancellationToken)
                    .ConfigureAwait(false);

                return pipeStream;
            }
            catch
            {
                await pipeStream
                    .DisposeAsync()
                    .ConfigureAwait(false);

                throw;
            }
        }

        private static NamedPipeClientStream CreateNamedPipeClientStream(string pipeName, string serverName)
        {
            return new NamedPipeClientStream(
                serverName,
                pipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }
    }
}