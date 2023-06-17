using System.IO.Pipes;

namespace AllOverIt.Pipes.Anonymous
{
    public sealed class AnonymousPipeClient : AnonymousPipeBase
    {
        private AnonymousPipeClientStream _pipeClientStream;

        public void Start(string clientHandle)
        {
            Start(PipeDirection.In, clientHandle);
        }

        public void Start(PipeDirection direction, string clientHandle)
        {
            _pipeClientStream = new AnonymousPipeClientStream(direction, clientHandle);

            InitializeStart(direction, _pipeClientStream);
        }
    }
}
