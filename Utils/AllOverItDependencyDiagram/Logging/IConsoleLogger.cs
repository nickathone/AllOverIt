using System;

namespace AllOverItDependencyDiagram.Logging
{
    internal interface IConsoleLogger
    {
        IConsoleLogger WriteFormatted(string formattedText);
        IConsoleLogger WriteFormattedLine(string formattedText);
        IConsoleLogger WriteFragment(ConsoleColor foreColor, string text);
        IConsoleLogger WriteFragment(ConsoleColor foreColor, ConsoleColor backColor, string text);
        IConsoleLogger WriteLine(ConsoleColor foreColor, string text);
        IConsoleLogger WriteLine(ConsoleColor foreColor, ConsoleColor backColor, string text);
        IConsoleLogger WriteLine();
    }
}