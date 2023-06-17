using System.IO;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Anonymous
{
    public sealed class AnonymousPipeServer : AnonymousPipeBase
    {
        private AnonymousPipeServerStream _pipeServerStream;
        private string _clientHandleString;
        private HandleInheritability _inheritability;

        public string Start(PipeDirection direction = PipeDirection.Out, HandleInheritability inheritability = HandleInheritability.None)
        {
            _inheritability = inheritability;

            _pipeServerStream = new AnonymousPipeServerStream(direction, _inheritability);

            InitializeStart(direction, _pipeServerStream);

            _clientHandleString = _pipeServerStream.GetClientHandleAsString();

            return _clientHandleString;
        }

        protected override void Dispose(bool disposing)
        {
            DisposeClientHandleIfRequired();

            base.Dispose(disposing);
        }

        private void DisposeClientHandleIfRequired()
        {
            if (_clientHandleString is not null)
            {
                _pipeServerStream.DisposeLocalCopyOfClientHandle();
                _clientHandleString = null;
            }
        }
    }
}
