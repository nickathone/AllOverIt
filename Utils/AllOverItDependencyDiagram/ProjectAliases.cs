using System.Collections.Generic;

namespace SolutionInspector
{
    internal enum AliasType
    {
        Project,            // Explicit project reference
        Package,            // Explicit package reference
        Transitive          // Implicit package reference
    }

    internal record ProjectAlias(string DisplayName, AliasType AliasType);

    // AllAliases contains a mapping of an alias to its' display name, such as: alloverit-assertion: AllOverIt.Assertion
    // AllDependencies contains all project aliases and their associated dependency aliases
    internal record ProjectAliases(IDictionary<string, ProjectAlias> AllAliases, HashSet<ProjectDependency> AllDependencies);
}