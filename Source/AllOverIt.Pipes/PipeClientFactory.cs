using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes
{
    internal static class PipeClientFactory
    {
        public static async Task<PipeReaderWriter> ConnectAsync(
            string pipeName,
            string serverName,
            //Func<string, string, NamedPipeClientStream> func = null,
            CancellationToken cancellationToken = default)
        {
            var pipe = await CreateAndConnectAsync(pipeName, serverName, /*func,*/ cancellationToken).ConfigureAwait(false);

            return new PipeReaderWriter(pipe);
        }

        public static async Task<NamedPipeClientStream> CreateAndConnectAsync(
            string pipeName,
            string serverName,
            //Func<string, string, NamedPipeClientStream> func = null,
            CancellationToken cancellationToken = default)
        {
            var pipe = //func != null
                       //? func(pipeName, serverName) :
                Create(pipeName, serverName);

            try
            {
                await pipe.ConnectAsync(cancellationToken).ConfigureAwait(false);

                return pipe;
            }
            catch
            {
                await pipe.DisposeAsync().ConfigureAwait(false);

                throw;
            }
        }

        public static NamedPipeClientStream Create(string pipeName, string serverName)
        {
            return new NamedPipeClientStream(
                serverName,
                pipeName,
                direction: PipeDirection.InOut,
                options: PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }
    }
}