using AllOverIt.Assertion;
using AllOverIt.Pipes.Named.Exceptions;
using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Connection
{
    public sealed class PipeReaderWriter : IAsyncDisposable
    {
        private readonly bool _leaveConnected;
        private readonly PipeStreamReader _streamReader;
        private readonly PipeStreamWriter _streamWriter;
        private PipeStream _pipeStream;


        /// <summary>
        ///     Gets a value indicating whether the <see cref="_pipeStream"/> object is connected or not.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the <see cref="_pipeStream"/> object is connected; otherwise, <c>false</c>.
        /// </returns>
        //public bool IsConnected => _pipeStream.IsConnected; // _streamReader.IsConnected and _streamWriter.IsConnected


        /// <summary>
        ///     Gets a value indicating whether the current stream supports read operations.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the stream supports read operations; otherwise, <c>false</c>.
        /// </returns>
        public bool CanRead => _pipeStream.CanRead;

        /// <summary>
        ///     Gets a value indicating whether the current stream supports write operations.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the stream supports write operations; otherwise, <c>false</c>.
        /// </returns>
        public bool CanWrite => _pipeStream.CanWrite;

        /// <summary>
        /// Constructs a new <c>PipeStreamWrapper</c> object that reads from and writes to the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from and write to</param>
        /// <param name="leaveConnected">When the <see cref="PipeReaderWriter"/> is disposed, the stream will be disposed when
        /// <see langword="false"/> otherwise it will remain connected.</param>
        public PipeReaderWriter(PipeStream stream, bool leaveConnected)
        {
            _pipeStream = stream.WhenNotNull(nameof(stream));
            _leaveConnected = leaveConnected;

            _streamReader = new PipeStreamReader(_pipeStream);
            _streamWriter = new PipeStreamWriter(_pipeStream);
        }


        /// <summary>
        /// Reads the next object from the pipe. 
        /// </summary>
        /// <returns>The next object read from the pipe, or <c>null</c> if the pipe disconnected.</returns>
        public Task<byte[]> ReadAsync(CancellationToken cancellationToken = default)
        {
            if (!_pipeStream.IsConnected)
            {
                throw new NotConnectedException("The pipe is not connected.");
            }

            return _streamReader.ReadAsync(cancellationToken);
        }

        /// <summary>
        /// Writes an object to the pipe.  This method blocks until all data is sent.
        /// </summary>
        /// <param name="buffer">Object to write to the pipe</param>
        /// <param name="cancellationToken"></param>
        public Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            if (!_pipeStream.IsConnected)
            {
                throw new InvalidOperationException("The pipe is not connected.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Writes, flushes, and waits for the pipe to drain
            return _streamWriter.WriteAsync(buffer, cancellationToken);
        }



        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (!_leaveConnected && _pipeStream is not null)
            {
                await _pipeStream.DisposeAsync().ConfigureAwait(false);
                _pipeStream = null;
            }

            //if (_streamReader is not null)
            //{
            //    await _streamReader.DisposeAsync().ConfigureAwait(false);
            //    _streamReader = null;
            //}

            //if (_streamWriter is not null)
            //{
            //    await _streamWriter.DisposeAsync().ConfigureAwait(false);
            //    _streamWriter = null;
            //}
        }
    }
}