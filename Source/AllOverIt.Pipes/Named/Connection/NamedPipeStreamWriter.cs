using AllOverIt.Assertion;
using AllOverIt.Threading;
using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Connection
{
    /// <summary>Provides the ability to write buffered data to an underlying pipe stream.</summary>
    public sealed class NamedPipeStreamWriter
    {
        private PipeStream _pipeStream;
        private readonly AwaitableLock _lock = new();

        private bool IsConnected => _pipeStream?.IsConnected ?? false;

        /// <summary>Constructor.</summary>
        /// <param name="pipeStream">The pipe stream to write to.</param>
        public NamedPipeStreamWriter(PipeStream pipeStream)
        {
            _pipeStream = pipeStream.WhenNotNull(nameof(pipeStream));
        }

        /// <summary>Writes an array of bytes to the underlying pipe stream. All data is prefixed with the length
        /// (converted from host order to network byte order) of the message. This method is thread safe, meaning
        /// multiple threads can safely write to the pipe stream, but an individual thread cannot call this method
        /// a second time while it still has a lock from a previous call. Such a scenario can occur when a task
        /// begins a write operation but the continuation occurs on another thread. If the first thread is re-used
        /// (such as in a fast asynchronous polling operation) before the continuation completes then the internal
        /// lock will become deadlocked due to the asynchronous lock not being capable of enforcing thread or task
        /// identity. If this scenario is encountered, the underyling pipe stream will intentionally be disconnected
        /// to allow for recovery.</summary>
        /// <param name="buffer">Byte data to write to the underlying pipe stream.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            _ = buffer.WhenNotNull(nameof(buffer));

            cancellationToken.ThrowIfCancellationRequested();

            if (!IsConnected)
            {
                return;
            }

            // TODO: Make the timeout period configurable
            // Using a timeout to detect a deadlock due to the semaphore not being thread aware and hence non re-entrant).
            var success = await _lock.TryEnterLockAsync(500, cancellationToken).ConfigureAwait(false);

            if (!success)
            {
                // Kill the underlying connection so the other end of the pipe will abort reading from the pipe
                // (PipeStreamReader.ReadAsync() will return 0 bytes).
                _pipeStream?.Dispose();
                _pipeStream = null;
            }

            // Double-check locking pattern (and detects the above disconnection)
            if (!IsConnected)
            {
                return;
            }

            try
            {
                await WriteLengthAsync(buffer.Length, cancellationToken).ConfigureAwait(false);

                await _pipeStream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);

                await _pipeStream.FlushAsync(cancellationToken).ConfigureAwait(false);

                _pipeStream.WaitForPipeDrain();
            }
            catch (IOException)
            {
                // Thrown if the pipe is broken due to the server terminating
            }
            finally
            {
                _lock.ExitLock();
            }
        }

        private ValueTask WriteLengthAsync(int length, CancellationToken cancellationToken)
        {
            var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));

            return _pipeStream.WriteAsync(buffer, cancellationToken);
        }
    }
}