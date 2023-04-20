using System;

namespace AllOverItDependencyDiagram.Logging
{
    internal interface IConsoleLogger
    {
        ConsoleLineOutputBuilder AddFragment(ConsoleColor consoleColor, string text);
    }
}