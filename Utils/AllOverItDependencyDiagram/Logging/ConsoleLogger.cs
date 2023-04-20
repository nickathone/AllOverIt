using System;

namespace AllOverItDependencyDiagram.Logging
{
    internal sealed class ConsoleLogger
    {
        public static void Log(ConsoleLineOutput output)
        {
            var foreground = Console.ForegroundColor;

            try
            {
                foreach (var fragment in output.Fragments)
                {
                    Console.ForegroundColor = fragment.ConsoleColor;
                    Console.Write(fragment.Text);
                }

                if (output.AppendLineBreak)
                {
                    Console.WriteLine();
                }
            }
            finally
            {
                Console.ForegroundColor = foreground;
            }
        }
    }
}