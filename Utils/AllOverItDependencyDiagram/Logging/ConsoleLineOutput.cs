using System.Collections.Generic;

namespace AllOverItDependencyDiagram.Logging
{
    internal record ConsoleLineOutput(IEnumerable<ConsoleLineFragment> Fragments, bool AppendLineBreak);
}