using AllOverIt.Pipes.Anonymous;
using System;

namespace AnonymousPipeClientDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client Started");

            if (args.Length > 0)
            {
                using (var pipeClient = new AnonymousPipeClient())
                {
                    pipeClient.Start(args[0]);

                    // Confirm for handshake message from the server.
                    Console.WriteLine("Client waiting for handshake...");

                    var message = pipeClient.Reader.ReadLine();

                    if (message != "Handshake")
                    {
                        Console.WriteLine("Invalid handshake message received");
                    }
                    else
                    {
                        // Read the server message and echo to the console - until 'quit' is received
                        do
                        {
                            message = pipeClient.Reader.ReadLine();
                            Console.WriteLine($"Client received: {message}");
                        } while (!message.Equals("quit", StringComparison.InvariantCultureIgnoreCase));
                    }
                }

                Console.WriteLine("Client terminated");
            }
        }
    }
}
