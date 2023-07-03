using AllOverIt.Assertion;
using AllOverIt.Pipes.Exceptions;
using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Connection
{
    // Composes a reader and writer
    internal sealed class NamedPipeReaderWriter : IAsyncDisposable
    {
        private readonly bool _leaveConnected;
        private readonly NamedPipeStreamReader _streamReader;
        private readonly NamedPipeStreamWriter _streamWriter;
        private PipeStream _pipeStream;

        public bool CanRead => _pipeStream.CanRead;
        public bool CanWrite => _pipeStream.CanWrite;

        public NamedPipeReaderWriter(PipeStream stream, bool leaveConnected)
        {
            _pipeStream = stream.WhenNotNull(nameof(stream));
            _leaveConnected = leaveConnected;

            _streamReader = new NamedPipeStreamReader(_pipeStream);
            _streamWriter = new NamedPipeStreamWriter(_pipeStream);
        }

        public Task<byte[]> ReadAsync(CancellationToken cancellationToken = default)
        {
            // Not checking if the connection is active here since it can be broken when further down the line.
            // Making the caller resposible for ensuring read/write operations are performed correctly.

            return _streamReader.ReadAsync(cancellationToken);
        }

        public Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            // Not checking if the connection is active here since it can be broken when further down the line.
            // Making the caller resposible for ensuring read/write operations are performed correctly.

            cancellationToken.ThrowIfCancellationRequested();

            // Writes, flushes, and waits for the pipe to drain
            return _streamWriter.WriteAsync(buffer, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_leaveConnected && _pipeStream is not null)
            {
                await _pipeStream.DisposeAsync().ConfigureAwait(false);
                _pipeStream = null;
            }
        }
    }
}