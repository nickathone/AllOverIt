using System;
using System.Threading.Tasks;

namespace NamedPipeDemo
{
    internal class Program
    {
        static async Task Main()
        {
            var pipeName = "named_pipe_test_server";

            await PipeClient.RunAsync(pipeName).ConfigureAwait(false);

            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}