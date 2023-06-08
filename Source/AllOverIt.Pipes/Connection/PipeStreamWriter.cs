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
    public sealed class PipeStreamWriter //: IAsyncDisposable
    {
        private PipeStream _pipeStream;
        private SemaphoreSlim _semaphoreSlim = new(1, 1);

        //public bool IsConnected => _semaphoreSlim is not null && _pipeStream.IsConnected;

        /// <summary>Constructor.</summary>
        /// <param name="stream">The pipe stream to write to.</param>
        public PipeStreamWriter(PipeStream stream)      // only wraps the stream, does not assume ownership
        {
            _pipeStream = stream.WhenNotNull(nameof(stream));
        }

        /// <summary>Writes an array of bytes to the pipe.</summary>
        /// <param name="buffer">Object to write to the pipe</param>
        /// <param name="cancellationToken"></param>
        public async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            _ = buffer.WhenNotNull(nameof(buffer));

            try
            {
                using (await _semaphoreSlim.DisposableWaitAsync(cancellationToken).ConfigureAwait(false))
                {
                    await WriteLengthAsync(buffer.Length, cancellationToken).ConfigureAwait(false);

                    await _pipeStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);

                    // Flush all buffers to the underlying device
                    await _pipeStream.FlushAsync(cancellationToken).ConfigureAwait(false);

                    // Wait for the other end to read all sent bytes
                    _pipeStream.WaitForPipeDrain();
                }
            }
            catch (IOException)
            {
                // Thrown if the pipe is broken due to the server terminating
            }
        }

        ///// <inheritdoc />
        //public async ValueTask DisposeAsync()
        //{
        //    if (_semaphoreSlim is not null)
        //    {
        //        using (await _semaphoreSlim.DisposableWaitAsync(CancellationToken.None).ConfigureAwait(false))
        //        {
        //            await _pipeStream.DisposeAsync().ConfigureAwait(false);
        //            _pipeStream = null;
        //        }

        //        _semaphoreSlim?.Dispose();
        //        _semaphoreSlim = null;
        //    }
        //}


        private async Task WriteLengthAsync(int length, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));

            await _pipeStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        }
    }
}