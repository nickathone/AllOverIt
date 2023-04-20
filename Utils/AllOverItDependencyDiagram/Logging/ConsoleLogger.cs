using System;

namespace AllOverItDependencyDiagram.Logging
{
    internal class ConsoleLogger : IConsoleLogger
    {
        public ConsoleLineOutputBuilder AddFragment(ConsoleColor consoleColor, string text)
        {
            return new ConsoleLineOutputBuilder(consoleColor, text, LogToConsole);
        }

        private static void LogToConsole(ConsoleLineOutput output)
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