using System.IO.Pipes;

namespace AllOverIt.Pipes.Server
{
    public static class PipeServerFactory
    {
        public static NamedPipeServerStream CreateNamedPipeServerStream(string pipeName, PipeSecurity pipeSecurity = default)
        {
            return NamedPipeServerStreamAcl.Create(
                pipeName: pipeName,
                direction: PipeDirection.InOut,
                maxNumberOfServerInstances: 1,
                transmissionMode: PipeTransmissionMode.Byte,
                options: PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                inBufferSize: 0,
                outBufferSize: 0,
                pipeSecurity: pipeSecurity,
                inheritability: System.IO.HandleInheritability.None,
                additionalAccessRights: default);
        }
    }
}