using System.IO.Pipes;

namespace AllOverIt.Pipes
{
    public static class PipeServerFactory
    {
        //public static async Task<NamedPipeServerStream> CreateAndWaitAsync(string pipeName, CancellationToken cancellationToken = default)
        //{
        //    var pipe = Create(pipeName);

        //    try
        //    {
        //        await pipe.WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);

        //        return pipe;
        //    }
        //    catch
        //    {
        //        await pipe.DisposeAsync().ConfigureAwait(false);

        //        throw;
        //    }
        //}

        public static NamedPipeServerStream Create(string pipeName)
        {
            return new NamedPipeServerStream(
                pipeName: pipeName,
                direction: PipeDirection.InOut,
                maxNumberOfServerInstances: 1,                                  // System.IO.Pipes.NamedPipeServerStream.MaxAllowedServerInstances = -1
                transmissionMode: PipeTransmissionMode.Byte,
                options: PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                inBufferSize: 0,
                outBufferSize: 0);
        }
    }


}