using System.Collections.Generic;

namespace SolutionInspector.Parser
{
    internal sealed class PackageReference
    {
        public string Name { get; init; }
        public string Version { get; init; }
        public IReadOnlyCollection<PackageReference> TransitiveReferences { get; init; }
    }
}