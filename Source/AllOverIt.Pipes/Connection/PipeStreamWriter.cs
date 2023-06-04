using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Connection
{
    public sealed class PipeStreamWriter : IAsyncDisposable
    {
        private PipeStream _pipeStream;
        private SemaphoreSlim _semaphoreSlim = new(1, 1);


        /// <summary>
        /// Constructs a new <c>PipeStreamWriter</c> object that writes to given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Pipe to write to</param>
        public PipeStreamWriter(PipeStream stream)
        {
            _pipeStream = stream.WhenNotNull(nameof(stream));
        }

        /// <summary>
        /// Writes an object to the pipe.
        /// </summary>
        /// <param name="buffer">Object to write to the pipe</param>
        /// <param name="cancellationToken"></param>
        public async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            buffer = buffer.WhenNotNull(nameof(buffer));

            using (await _semaphoreSlim.DisposableWaitAsync(cancellationToken))
            {
                await WriteLengthAsync(buffer.Length, cancellationToken).ConfigureAwait(false);

                await _pipeStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);

                try
                {
                    // Flush all buffers to the underlying device
                    await _pipeStream.FlushAsync(cancellationToken).ConfigureAwait(false);

                    // Wait for the other end to read all sent bytes
                    _pipeStream.WaitForPipeDrain();
                }
                catch (IOException exception)
                {
                    // Ignore: https://stackoverflow.com/questions/45308306/gracefully-closing-a-named-pipe-and-disposing-of-streams
                }
            }
        }


        /// <summary>
        /// Dispose internal <see cref="PipeStream"/>
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_pipeStream is not null)
            {
                await _pipeStream.DisposeAsync().ConfigureAwait(false);
                _pipeStream = null;
            }

            _semaphoreSlim?.Dispose();
            _semaphoreSlim = null;
        }


        private async Task WriteLengthAsync(int length, CancellationToken cancellationToken)
        {
            var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));

            await _pipeStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        }
    }


}