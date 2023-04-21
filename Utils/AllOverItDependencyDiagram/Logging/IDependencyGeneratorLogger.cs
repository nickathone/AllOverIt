using System;

namespace AllOverItDependencyDiagram.Logging
{
    internal interface IDependencyGeneratorLogger
    {
        IDependencyGeneratorLogger WriteFormatted(string formattedText);
        IDependencyGeneratorLogger WriteFormattedLine(string formattedText);
        IDependencyGeneratorLogger WriteFragment(ConsoleColor foreColor, string text);
        IDependencyGeneratorLogger WriteFragment(ConsoleColor foreColor, ConsoleColor backColor, string text);
        IDependencyGeneratorLogger WriteLine(ConsoleColor foreColor, string text);
        IDependencyGeneratorLogger WriteLine(ConsoleColor foreColor, ConsoleColor backColor, string text);
        IDependencyGeneratorLogger WriteLine();
    }
}