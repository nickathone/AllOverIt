using AllOverIt.Assertion;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes
{
    public sealed class PipeStreamReader : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the pipe is connected or not.
        /// </summary>
        public bool IsConnected { get; private set; }

        private PipeStream BaseStream { get; }



        /// <summary>
        /// Constructs a new <c>PipeStreamReader</c> object that reads data from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Pipe to read from</param>
        public PipeStreamReader(PipeStream stream)
        {
            BaseStream = stream.WhenNotNull(nameof(stream));
            IsConnected = stream.IsConnected;
        }




        /// <summary>
        /// Reads the next object from the pipe.  This method waits until an object is sent
        /// or the pipe is disconnected.
        /// </summary>
        /// <returns>The next object read from the pipe, or <c>null</c> if the pipe disconnected.</returns>
        public async Task<byte[]> ReadAsync(CancellationToken cancellationToken)
        {
            var length = await ReadLengthAsync(cancellationToken).ConfigureAwait(false);

            return length == 0
                ? default
                : await ReadAsync(length, true, cancellationToken).ConfigureAwait(false);
        }



        public void Dispose()
        {
            BaseStream.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await BaseStream.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the length of the next message (in bytes) from the client.
        /// </summary>
        /// <returns>Number of bytes of data the client will be sending.</returns>
        /// <exception cref="InvalidOperationException">The pipe is disconnected, waiting to connect, or the handle has not been set.</exception>
        /// <exception cref="IOException">Any I/O error occurred.</exception>
        private async Task<int> ReadLengthAsync(CancellationToken cancellationToken)
        {
            var bytes = await ReadAsync(sizeof(int), false, cancellationToken).ConfigureAwait(false);

            if (!bytes.Any())
            {
                IsConnected = false;
                return 0;
            }

            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
        }

        private async Task<byte[]> ReadAsync(int length, bool throwIfReadLessThanLength, CancellationToken cancellationToken)
        {
            var buffer = new byte[length];

            var read = await BaseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            if (read != buffer.Length)
            {
                return throwIfReadLessThanLength
                    ? throw new IOException($"Expected {buffer.Length} bytes but read {read}")
                    : Array.Empty<byte>();
            }

            return buffer;
        }
    }


}