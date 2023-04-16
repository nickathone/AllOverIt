using System.Collections.Generic;

namespace SolutionInspector
{
    // AllAliases contains a mapping of an alias to its' display name, such as: alloverit-assertion: AllOverIt.Assertion
    // AllDependencies contains all project aliases and their associated dependency aliases
    internal record ProjectAliases(IDictionary<string, string> AllAliases, HashSet<ProjectDependency> AllDependencies);
}