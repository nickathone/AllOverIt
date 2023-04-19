using System.Collections.Generic;

namespace SolutionInspector.Parser
{
    internal sealed class PackageReference
    {
        public bool IsTransitive { get; }
        public string Name { get; init; }
        public string Version { get; init; }
        public IReadOnlyCollection<PackageReference> TransitiveReferences { get; init; }

        public PackageReference(bool isTransitive = false)
        {
            IsTransitive = isTransitive;
        }
    }
}