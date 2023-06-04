using System.IO.Pipes;

namespace AllOverIt.Pipes.Server
{
    public static class PipeServerFactory
    {
        public static NamedPipeServerStream Create(string pipeName, PipeSecurity pipeSecurity = null)
        {
            return NamedPipeServerStreamAcl.Create(
                pipeName: pipeName,
                direction: PipeDirection.InOut,
                maxNumberOfServerInstances: 1,
                transmissionMode: PipeTransmissionMode.Byte,
                options: PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                inBufferSize: 0,
                outBufferSize: 0,
                pipeSecurity: pipeSecurity);
        }
    }
}