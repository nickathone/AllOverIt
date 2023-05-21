using AllOverIt.Assertion;
using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes
{
    public sealed class PipeReaderWriter : /*IDisposable,*/ IAsyncDisposable
    {
        private readonly PipeStream _pipeStream;
        private readonly PipeStreamReader _streamReader;
        private readonly PipeStreamWriter _streamWriter;


        /// <summary>
        ///     Gets a value indicating whether the <see cref="_pipeStream"/> object is connected or not.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the <see cref="_pipeStream"/> object is connected; otherwise, <c>false</c>.
        /// </returns>
        public bool IsConnected => _pipeStream.IsConnected && _streamReader.IsConnected;

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
        /// <exception cref="ArgumentNullException"></exception>
        public PipeReaderWriter(PipeStream stream)
        {
            _pipeStream = stream.WhenNotNull(nameof(stream));

            _streamReader = new PipeStreamReader(_pipeStream);
            _streamWriter = new PipeStreamWriter(_pipeStream);
        }


        /// <summary>
        /// Reads the next object from the pipe. 
        /// </summary>
        /// <returns>The next object read from the pipe, or <c>null</c> if the pipe disconnected.</returns>
        public Task<byte[]> ReadAsync(CancellationToken cancellationToken = default)
        {
            return _streamReader.ReadAsync(cancellationToken);
        }

        /// <summary>
        /// Writes an object to the pipe.  This method blocks until all data is sent.
        /// </summary>
        /// <param name="buffer">Object to write to the pipe</param>
        /// <param name="cancellationToken"></param>
        public Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            // Writes, flushes, and waits for the pipe to drain
            return _streamWriter.WriteAsync(buffer, cancellationToken);
        }



        /// <summary>
        /// Dispose internal <see cref="PipeStream"/>
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await _pipeStream.DisposeAsync().ConfigureAwait(false);

            // TODO: Check this - they seem to dispose the same reference as BaseStream
            //
            // This is redundant, just to avoid mistakes and follow the general logic of Dispose
            await _streamReader.DisposeAsync().ConfigureAwait(false);
            await _streamWriter.DisposeAsync().ConfigureAwait(false);
        }
    }


}