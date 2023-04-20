using System.Collections.Generic;

namespace SolutionInspector.Parser
{
    internal sealed class PackageReference
    {
        public bool IsTransitive { get; }
        public int Depth { get; }
        public string Name { get; init; }
        public string Version { get; init; }
        public IReadOnlyCollection<PackageReference> TransitiveReferences { get; init; }

        public PackageReference()
            : this(false, 0)
        {
        }

        public PackageReference(bool isTransitive, int depth)
        {
            IsTransitive = isTransitive;
            Depth = depth;
        }
    }
}