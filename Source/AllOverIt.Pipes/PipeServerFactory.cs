using System.IO.Pipes;

namespace AllOverIt.Pipes
{
    public static class PipeServerFactory
    {
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static NamedPipeServerStream Create(string pipeName, PipeSecurity pipeSecurity = null)
        {

            // public static NamedPipeServerStream Create(
            //   string pipeName,
            //   PipeDirection direction,
            //   int maxNumberOfServerInstances,
            //   PipeTransmissionMode transmissionMode,
            //   PipeOptions options, int inBufferSize,
            //   int outBufferSize,
            //   PipeSecurity? pipeSecurity,
            //   HandleInheritability inheritability = HandleInheritability.None,
            //   PipeAccessRights additionalAccessRights = 0);


            return NamedPipeServerStreamAcl.Create(
                pipeName: pipeName,
                direction: PipeDirection.InOut,
                maxNumberOfServerInstances: 1,
                transmissionMode: PipeTransmissionMode.Byte,
                options: PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                inBufferSize: 0,
                outBufferSize: 0,
                pipeSecurity: pipeSecurity);


            //return new NamedPipeServerStream(
            //    pipeName: pipeName,
            //    direction: PipeDirection.InOut,
            //    maxNumberOfServerInstances: 1,
            //    transmissionMode: PipeTransmissionMode.Byte,
            //    options: PipeOptions.Asynchronous | PipeOptions.WriteThrough,
            //    inBufferSize: 0,
            //    outBufferSize: 0);
        }
    }
}