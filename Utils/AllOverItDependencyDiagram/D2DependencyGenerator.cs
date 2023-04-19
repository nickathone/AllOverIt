using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using SolutionInspector.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionInspector
{
    internal static class D2DependencyGenerator
    {
        public static async Task CreateAllFilesAsync(string solutionPath, string projectsRootPath, string docsPath, Action<ProjectAliases> reportDiagramData)
        {
            // The paths are required to work out dependency project absolute paths from their relative paths
            _ = solutionPath.WhenNotNullOrEmpty();
            _ = projectsRootPath.WhenNotNullOrEmpty();
            _ = docsPath.WhenNotNullOrEmpty();

            var projectAliases = await GetProjectAliasesAsync(solutionPath, projectsRootPath);

            reportDiagramData.Invoke(projectAliases);

            var scopes = new[] { string.Empty }     // everything
                .Concat(projectAliases.AllAliases.Where(item => item.Key.StartsWith("alloverit"))
                .SelectAsReadOnlyCollection(item => item.Key));

            var extensions = new[] { "svg", "png" /*, "pdf"*/ };

            foreach (var scope in scopes)
            {
                // Get the D2 diagram content
                var d2Content = GenerateD2Content(projectAliases, scope);

                // Create the file and return the fully-qualified file path
                var filePath = await CreateD2FileAsync(d2Content, docsPath, scope);

                foreach (var extension in extensions)
                {
                    await ExportD2ImageFileAsync(filePath, extension);
                }
            }
        }

        private static async Task<ProjectAliases> GetProjectAliasesAsync(string solutionPath, string projectsRootPath)
        {
            var allAliases = new Dictionary<string, ProjectAlias>();
            var allDependencies = new HashSet<ProjectDependency>();

            var projectAliases = new ProjectAliases(allAliases, allDependencies);

            var solutionParser = new SolutionParser();
            var projects = await solutionParser.ParseAsync(solutionPath, projectsRootPath);

            foreach (var project in projects)
            {
                var projectAlias = project.Name.Replace(".", "-").ToLowerInvariant();

                UpdateAliasCache(projectAlias, project.Name, AliasType.Project, projectAliases.AllAliases);
                AddProjectDependencies(projectAlias, project.Dependencies, projectAliases);
                AddPackageDependencies(projectAlias, project.Dependencies, projectAliases);
            }

            return projectAliases;
        }

        private static void AddProjectDependencies(string projectAlias, IReadOnlyCollection<ConditionalReferences> projectDependencies, ProjectAliases projectAliases)
        {
            var allProjectDependencies = projectDependencies.SelectMany(dependency => dependency.ProjectReferences);

            foreach (var projectDependency in allProjectDependencies)
            {
                var projectDependencyName = Path.GetFileNameWithoutExtension(projectDependency.Path);

                var projectDependencyAlias = projectDependencyName.Replace(".", "-").ToLowerInvariant();

                UpdateAliasCache(projectDependencyAlias, projectDependencyName, AliasType.Project, projectAliases.AllAliases);

                var dependency = new ProjectDependency(projectAlias, projectDependencyAlias);
                projectAliases.AllDependencies.Add(dependency);
            }
        }

        private static void AddPackageDependencies(string parentAlias, IReadOnlyCollection<ConditionalReferences> projectDependencies, ProjectAliases projectAliases)
        {
            var allPackageDependencies = projectDependencies.SelectMany(dependency => dependency.PackageReferences);

            foreach (var packageDependency in allPackageDependencies)
            {
                ProcessPackageReference(parentAlias, packageDependency, projectAliases);
            }
        }

        private static void ProcessPackageReference(string alias, PackageReference packageDependency, ProjectAliases projectAliases)
        {
            var packageDependencyName = packageDependency.Name;

            var packageDependencyAlias = packageDependencyName.Replace(".", "-").ToLowerInvariant();

            var aliasType = packageDependency.IsTransitive
                ? AliasType.Transitive
                : AliasType.Package;

            UpdateAliasCache(packageDependencyAlias, packageDependencyName, aliasType, projectAliases.AllAliases);

            var dependency = new ProjectDependency(alias, packageDependencyAlias);

            // Only add the dependency if it hasn't been seen before. Note, 'dependency' is a record type.
            if (projectAliases.AllDependencies.Add(dependency))
            {
                foreach (var transitive in packageDependency.TransitiveReferences)
                {
                    ProcessPackageReference(packageDependencyAlias, transitive, projectAliases);
                }
            }
        }

        private static string GenerateD2Content(ProjectAliases projectAliases, string scope = default)
        {
            var sb = new StringBuilder();

            sb.AppendLine("direction: right");
            sb.AppendLine();

            sb.AppendLine($"aoi: AllOverIt");

            var allAliases = projectAliases.AllAliases;
            var allDependencies = projectAliases.AllDependencies.AsEnumerable();

            var aliasesUsed = allAliases;

            if (scope.IsNotNullOrEmpty())
            {
                IEnumerable<ProjectDependency> GetProjectDependencies(string alias)
                {
                    var dependencies = projectAliases.AllDependencies.Where(item => item.Alias == alias);

                    foreach (var dependency in dependencies)
                    {
                        yield return dependency;

                        foreach (var dependencyAlias in GetProjectDependencies(dependency.DependencyAlias))
                        {
                            yield return dependencyAlias;
                        }
                    }
                }

                // Get all dependencies recursively
                allDependencies = GetProjectDependencies(scope).AsReadOnlyCollection();

                aliasesUsed = new Dictionary<string, ProjectAlias>();

                if (allDependencies.Any())
                {
                    foreach (var (alias, dependencyAlias) in allDependencies)
                    {
                        UpdateAliasCache(alias, allAliases[alias], aliasesUsed);
                        UpdateAliasCache(dependencyAlias, allAliases[dependencyAlias], aliasesUsed);
                    }
                }
                else
                {
                    // If there's no dependencies then add the scope itself - otherwise there'll be nothing to export
                    UpdateAliasCache(scope, allAliases[scope], aliasesUsed);
                }
            }

            foreach (var alias in aliasesUsed)
            {
                var projectAlias = alias.Value;

                sb.AppendLine($"{GetDiagramAliasId(alias.Key)}: {projectAlias.DisplayName}");

                // Style non-AllOverIt dependencies
                if (!alias.Key.StartsWith("alloverit"))
                {
                    var color = projectAlias.AliasType == AliasType.Package
                        ? "#ADD8E6"     // Blue
                        : "#FFEC96";    // Yellow

                    sb.AppendLine($"{alias.Key}.style.fill: \"{color}\"");
                    sb.AppendLine($"{alias.Key}.style.opacity: 0.8");
                }
            }

            sb.AppendLine();

            foreach (var (alias, dependencyAlias) in allDependencies)
            {
                sb.AppendLine($"{GetDiagramAliasId(dependencyAlias)} <- {GetDiagramAliasId(alias)}");
            }

            return sb.ToString();
        }

        private static void UpdateAliasCache(string alias, string name, AliasType aliasType, IDictionary<string, ProjectAlias> aliases)
        {
            if (aliases.TryGetValue(alias, out var projectAlias))
            {
                // Override transitive references with explicit package references (for the diagram output)
                if (projectAlias.AliasType == AliasType.Transitive && aliasType == AliasType.Package)
                {
                    projectAlias = new ProjectAlias(name, aliasType);
                }
            }
            else
            {
                projectAlias = new ProjectAlias(name, aliasType);
            }

            // Add / Replace as applicable
            aliases[alias] = projectAlias;
        }

        private static void UpdateAliasCache(string alias, ProjectAlias projectAlias, IDictionary<string, ProjectAlias> aliases)
        {
            if (!aliases.ContainsKey(alias))
            {
                aliases.Add(alias, projectAlias);
            }
        }

        private static async Task<string> CreateD2FileAsync(string content, string docsPath, string scope)
        {
            var fileName = scope.IsNullOrEmpty()
                ? "alloverit-all.d2"
                : $"{scope}.d2";

            var d2FilePath = Path.Combine(docsPath, fileName);

            Console.WriteLine($"Creating D2 diagram and images for '{Path.GetFileNameWithoutExtension(fileName)}'...");

            File.WriteAllText(d2FilePath, content);

            await ProcessBuilder
                .For("d2.exe")
                .WithArguments("fmt", d2FilePath)
                .BuildProcessExecutor()
                .ExecuteAsync();

            Console.WriteLine($"Created diagram {d2FilePath}");

            return d2FilePath;
        }

        private static async Task ExportD2ImageFileAsync(string d2FileName, string extension)
        {
            var imageFileName = Path.ChangeExtension(d2FileName, extension);

            var export = ProcessBuilder
               .For("d2.exe")
               .WithArguments("-l", "elk", d2FileName, imageFileName)
               .BuildProcessExecutor();

            await export.ExecuteAsync();

            Console.WriteLine($"Exported image {imageFileName}");
        }

        private static string GetDiagramAliasId(string alias)
        {
            // Group all 'AllOverIt' packages together
            return alias.StartsWith("alloverit")
                ? $"aoi.{alias}"
                : alias;
        }
    }
}