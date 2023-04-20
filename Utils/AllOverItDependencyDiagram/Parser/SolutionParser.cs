using AllOverIt.Extensions;
using AllOverIt.Io;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SolutionInspector.Parser
{
    internal sealed class SolutionParser
    {
        private readonly NugetPackageReferencesResolver _nugetResolver = new(1);

        public Task<IReadOnlyCollection<SolutionProject>> ParseAsync(string solutionFilePath, string projectIncludePath)
        {
            return GetProjectsAsync(solutionFilePath, projectIncludePath);
        }

        private async Task<IReadOnlyCollection<SolutionProject>> GetProjectsAsync(string solutionFilePath, string projectIncludePath)
        {
            var projects = new List<SolutionProject>();

            var solutionFile = SolutionFile.Parse(solutionFilePath);

            var orderedProjects = solutionFile.ProjectsInOrder
                .Where(project => project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
                .Where(project =>
                {
                    return project.AbsolutePath.StartsWith(projectIncludePath, StringComparison.OrdinalIgnoreCase);
                })
                .OrderBy(item => item.ProjectName);

            foreach (var projectItem in orderedProjects)
            {
                var projectRootElement = ProjectRootElement.Open(projectItem.AbsolutePath);
                var projectFolder = Path.GetDirectoryName(projectItem.AbsolutePath);

                var targetFrameworks = GetTargetFrameworks(projectRootElement.PropertyGroups);
                var conditionalReferences = await GetConditionalReferencesAsync(projectFolder, projectRootElement.ItemGroups).ToListAsync();

                var project = new SolutionProject
                {
                    Name = projectItem.ProjectName,
                    Path = projectItem.AbsolutePath,
                    TargetFrameworks = targetFrameworks,
                    Dependencies = conditionalReferences.AsReadOnlyCollection()
                };

                projects.Add(project);
            }

            return projects;
        }

        private static IReadOnlyCollection<string> GetTargetFrameworks(IEnumerable<ProjectPropertyGroupElement> propertyGroups)
        {
            return propertyGroups
                .SelectMany(grp => grp.Properties)
                .Where(prop => prop.Name.Equals("TargetFrameworks", StringComparison.OrdinalIgnoreCase) ||
                               prop.Name.Equals("TargetFramework", StringComparison.OrdinalIgnoreCase))
                .Select(prop => prop.Value)
                .Single()
                .Split(";");
        }

        private async IAsyncEnumerable<ConditionalReferences> GetConditionalReferencesAsync(string projectFolder, IEnumerable<ProjectItemGroupElement> itemGroups)
        {
            var conditionItemGroups = itemGroups
                .Select(grp => new
                {
                    grp.Condition,
                    grp.Items
                })
                .GroupBy(grp => grp.Condition);

            foreach (var itemGroup in conditionItemGroups)
            {
                var items = itemGroup.SelectMany(value => value.Items).ToList();

                var projectReferences = GetProjectReferences(projectFolder, items);
                var packageReferences = await GetPackageReferencesAsync(items);

                var conditionalReferences = new ConditionalReferences
                {
                    Condition = itemGroup.Key,
                    ProjectReferences = projectReferences,
                    PackageReferences = packageReferences
                };

                yield return conditionalReferences;
            }
        }

        private static IReadOnlyCollection<ProjectReference> GetProjectReferences(string projectFolder, IEnumerable<ProjectItemElement> projectItems)
        {
            return projectItems
                .Where(item => item.ItemType.Equals("ProjectReference", StringComparison.OrdinalIgnoreCase))
                .Select(item => 
                {
                    var projectPath = FileUtils.GetAbsolutePath(projectFolder, item.Include);

                    // This can be used if the project ever needs to be inspected:
                    // var projectRootElement = ProjectRootElement.Open(projectPath);

                    return new ProjectReference
                    {
                        Path = projectPath
                    };
                })
                .ToList();
        }

        private async Task<IReadOnlyCollection<PackageReference>> GetPackageReferencesAsync(IEnumerable<ProjectItemElement> projectItems)
        {
            var packageReferences = await projectItems
                .Where(item => item.ItemType.Equals("PackageReference", StringComparison.OrdinalIgnoreCase))
                .SelectAsync(async item =>
                {
                    var packageName = item.Include;
                    var packageVersion = item.Metadata.SingleOrDefault(item => item.Name == "Version")?.Value;
                    var transitivePackages = await _nugetResolver.GetPackageReferences(packageName, packageVersion);

                    return new PackageReference
                    {
                        Name = packageName,
                        Version = packageVersion,
                        TransitiveReferences = transitivePackages
                    };
                })
                .ToListAsync();

            return packageReferences.AsReadOnlyCollection();
        }
    }
}