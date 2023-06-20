using AllOverIt.Pipes.Named.Connection;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Client
{
    internal static class NamedPipeClientStreamFactory
    {
        public static async Task<NamedPipeReaderWriter> CreateConnectedReaderWriterAsync(string pipeName, string serverName, CancellationToken cancellationToken = default)
        {
            var pipeStream = await CreateConnectedClientStreamAsync(pipeName, serverName, cancellationToken).ConfigureAwait(false);

            return new NamedPipeReaderWriter(pipeStream, false);
        }

        public static async Task<NamedPipeClientStream> CreateConnectedClientStreamAsync(string pipeName, string serverName, CancellationToken cancellationToken = default)
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