using System;
using System.IO;
using System.IO.Pipes;
using AllOverIt.Assertion;

namespace AllOverIt.Pipes.Anonymous
{
    public abstract class AnonymousPipeBase : IDisposable
    {
        private PipeDirection _direction;
        private PipeStream _pipeStream;
        private StreamReader _reader;
        private StreamWriter _writer;

        public StreamReader Reader
        {
            get
            {
                AssertCanRead();

                _reader ??= new StreamReader(_pipeStream);

                return _reader;
            }
        }

        public StreamWriter Writer
        {
            get
            {
                AssertCanWrite();

                _writer ??= new StreamWriter(_pipeStream);

                return _writer;
            }
        }


        public void WaitForPipeDrain()
        {
            AssertCanWrite();

            _pipeStream.WaitForPipeDrain();
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected void InitializeStart(PipeDirection direction, PipeStream pipeStream)
        {
            // TODO: throw if already started

            _direction = direction;
            _pipeStream = pipeStream;

            if (_direction == PipeDirection.Out)
            {
                _writer = new StreamWriter(_pipeStream)
                {
                    AutoFlush = true
                };
            }
            else
            {
                _reader = new StreamReader(_pipeStream);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _reader?.Dispose();
                _reader = null;

                _writer?.Dispose();
                _writer = null;

                _pipeStream?.Dispose();
                _pipeStream = null;
            }
        }

        private void AssertCanRead()
        {
            Throw<InvalidOperationException>.When(_direction == PipeDirection.Out, "The anonymous pipe is write-only.");
        }

        private void AssertCanWrite()
        {
            Throw<InvalidOperationException>.When(_direction == PipeDirection.In, "The anonymous pipe is read-only.");
        }
    }
}
