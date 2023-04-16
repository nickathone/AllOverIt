using System.Collections.Generic;

namespace SolutionInspector.Parser
{
    internal sealed class SolutionProject
    {
        public string Name { get; init; }
        public string Path { get; init; }
        public IReadOnlyCollection<string> TargetFrameworks { get; init; }
        public IReadOnlyCollection<ConditionalReferences> Dependencies { get; init; }
    }
}