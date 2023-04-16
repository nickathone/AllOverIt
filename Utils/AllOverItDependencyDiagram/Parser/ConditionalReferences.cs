﻿using System.Collections.Generic;

namespace SolutionInspector.Parser
{
    internal sealed class ConditionalReferences
    {
        public string Condition { get; init; }
        public IReadOnlyCollection<ProjectReference> ProjectReferences { get; init; }
        public IReadOnlyCollection<PackageReference> PackageReferences { get; init; }
    }
}