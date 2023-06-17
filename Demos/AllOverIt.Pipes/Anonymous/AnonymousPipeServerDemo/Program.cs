using AllOverIt.IO;
using AllOverIt.Pipes.Anonymous;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace AnonymousPipeServerDemo
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Server Started");
            
            Process clientProcess;

            using (var pipeServer = new AnonymousPipeServer())
            {
                // Start the pipe server and get the client handle (the client app needs this to connect back to the server). 
                var clientHandle = pipeServer.Start(inheritability: HandleInheritability.Inheritable);

                // Determine the location of the client demo to launch
                var clientPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("Server", "Client");
                var clientExePath = Path.Combine(clientPath, "AnonymousPipeClientDemo.exe");

                Console.WriteLine("Launching client app...");

                // Launch the client application, giving it the client handle provided by the server.
                clientProcess = ProcessBuilder
                    .For(clientExePath)
                    .WithArguments(clientHandle)
                    .Start();

                try
                {
                    // Send a handshake message and wait for client to receive it.
                    pipeServer.Writer.WriteLine("Handshake");
                    pipeServer.WaitForPipeDrain();

                    string message;

                    do
                    {
                        Console.WriteLine("Enter text to send to the client: ");
                        message = Console.ReadLine();
                        pipeServer.Writer.WriteLine(message);
                    } while (!message.Equals("quit", StringComparison.InvariantCultureIgnoreCase));
                }
                catch (IOException e)
                {
                    //  raised if the pipe is broken or disconnected
                    Console.WriteLine($"Server Error: {e.Message}");
                }
            }

            clientProcess.WaitForExit();
            clientProcess.Close();

            Console.WriteLine("Server terminated");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}
