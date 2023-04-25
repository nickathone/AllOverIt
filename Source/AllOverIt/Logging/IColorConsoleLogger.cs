using System;

namespace AllOverIt.Logging
{
    public interface IColorConsoleLogger
    {
        IColorConsoleLogger WriteFormatted(string formattedText);
        IColorConsoleLogger WriteFormattedLine(string formattedText);
        IColorConsoleLogger WriteFragment(ConsoleColor foreColor, string text);
        IColorConsoleLogger WriteFragment(ConsoleColor foreColor, ConsoleColor backColor, string text);
        IColorConsoleLogger WriteLine(ConsoleColor foreColor, string text);
        IColorConsoleLogger WriteLine(ConsoleColor foreColor, ConsoleColor backColor, string text);
        IColorConsoleLogger Write(string text);
        IColorConsoleLogger WriteLine();
    }
}