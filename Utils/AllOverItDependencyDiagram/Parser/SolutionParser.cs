using AllOverIt.Extensions;
using AllOverIt.Io;
using Flurl.Http;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SolutionInspector.Parser
{
    internal sealed class SolutionParser
    {
        private readonly IDictionary<(string, string), IEnumerable<PackageReference>> _nugetCache = new Dictionary<(string, string), IEnumerable<PackageReference>>();

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

            _nugetCache.Clear();

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
                    var transitivePackages = await GetTransitivePackageReferencesAsync(packageName, packageVersion);

                    return new PackageReference
                    {
                        Name = packageName,
                        Version = packageVersion,
                        TransitiveReferences = transitivePackages.AsReadOnlyCollection()
                    };
                })
                .ToListAsync();

            return packageReferences.AsReadOnlyCollection();
        }

        private Task<IEnumerable<PackageReference>> GetTransitivePackageReferencesAsync(string packageName, string packageVersion)
        {
            return GetTransitivePackageReferencesRecursivelyAsync(packageName, packageVersion, 1);
        }

        private async Task<IEnumerable<PackageReference>> GetTransitivePackageReferencesRecursivelyAsync(string packageName, string packageVersion,
            int depth)
        {
            if (depth > 1)
            {
                return Array.Empty<PackageReference>();
            }

            var cacheKey = (packageName, packageVersion);
            
            if (!_nugetCache.TryGetValue(cacheKey, out var packageReferences))
            {
                if (packageVersion[0] == '[')
                {
                    // Need to handle versions such as [2.1.1, 3.0.0)
                    packageVersion = packageVersion[1..^1].Split(",").First().Trim();
                }

                var apiUrl = $"https://api.nuget.org/v3-flatcontainer/{packageName}/{packageVersion}/{packageName}.nuspec";
                var nuspecXml = await apiUrl.GetStringAsync();
                var nuspec = XDocument.Parse(nuspecXml);

                var ns = nuspec.Root.Name.Namespace;

                var dependenciesByFramework = nuspec.Descendants(ns + "group")
                    .Where(grp => grp.Attribute("targetFramework") != null) // ensure targetFramework attribute is present
                    .GroupBy(grp => grp.Attribute("targetFramework").Value)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Descendants(ns + "dependency")
                                  .SelectAsReadOnlyCollection(element => new
                                  {
                                      Id = element.Attribute("id").Value,
                                      Version = element.Attribute("version").Value
                                  })
                    );

                if (dependenciesByFramework.Any())
                {
                    var packageReferencesList = new List<PackageReference>();

                    // =====
                    // take the last for now - until everything is actually grouped (or constrained) by the target framework
                    // =====

                    foreach (var dependency in dependenciesByFramework.Last().Value)
                    {
                        var dependencyName = dependency.Id;
                        var dependencyVersion = dependency.Version;

                        var transitiveReferences = await GetTransitivePackageReferencesRecursivelyAsync(dependencyName, dependencyVersion, depth + 1);

                        var packageReference = new PackageReference(true)
                        {
                            Name = dependencyName,
                            Version = dependencyVersion,
                            TransitiveReferences = transitiveReferences.AsReadOnlyCollection()
                        };

                        packageReferencesList.Add(packageReference);
                    }

                    packageReferences = packageReferencesList;
                }

                _nugetCache.Add(cacheKey, packageReferences);
            }

            return packageReferences ?? Array.Empty<PackageReference>();
        }
    }
}