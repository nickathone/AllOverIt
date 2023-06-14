using AllOverIt.Logging;
using System;

namespace NamedPipeTypes
{
    public static class PipeLogger
    {
        private static readonly IColorConsoleLogger Logger = new ColorConsoleLogger();

        public static void Append(ConsoleColor color, string message)
        {
            lock (Logger)
            {
                Logger.WriteLine(color, message);
            }
        }
    }
}