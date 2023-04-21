using System;

namespace AllOverItDependencyDiagram.Logging
{
    internal interface IConsoleLogger
    {
        IConsoleLogger AddFormatted(string formattedText);
        IConsoleLogger AddFormattedLine(string formattedText);
        IConsoleLogger AddFragment(ConsoleColor foreColor, string text);
        IConsoleLogger AddFragment(ConsoleColor foreColor, ConsoleColor backColor, string text);
        IConsoleLogger AddLine(ConsoleColor foreColor, string text);
        IConsoleLogger AddLine(ConsoleColor foreColor, ConsoleColor backColor, string text);
        IConsoleLogger NewLine();
    }
}