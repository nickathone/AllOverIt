using AllOverIt.Extensions;
using AllOverIt.Io;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using SolutionInspector.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolutionInspector
{
    internal record ProjectDependency(string Alias, string DependencyAlias);
    internal record ProjectAliases(IDictionary<string, string> AllAliases, HashSet<ProjectDependency> AllDependencies);

    internal class Program
    {
        static async Task Main()
        {
            var applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var allOverItRoot = FileUtils.GetAbsolutePath(applicationPath, @"..\..\..\..\..\");
            var solutionPath = Path.Combine(allOverItRoot, "AllOverIt.sln");
            var projectsRootPath = Path.Combine(allOverItRoot, "Source");

            var projectAliases = GetProjectAliases(solutionPath, projectsRootPath);

            WriteDependenciesToConsole(projectAliases);

            var docsPath = Path.Combine(allOverItRoot, @"Docs\Dependencies");
            await CreateAllFilesAsync(projectAliases, docsPath);

            Console.WriteLine();
            Console.WriteLine($"AllOverIt.");
        }

        private static async Task CreateAllFilesAsync(ProjectAliases projectAliases, string docsPath)
        {
            var scopes = new[] { string.Empty }     // everything
                .Concat(projectAliases.AllAliases.Where(item => item.Key.StartsWith("alloverit"))
                .SelectAsReadOnlyCollection(item => item.Key));

            // Create all diagrams, then process the image exports in parallel 4 at a time
            // Could do it all sequentially, but showing how the partioned work can be performed
            var d2FilePaths = await scopes
                .SelectAsync(async scope =>
                {
                    // Get the D2 diagram content
                    var d2Content = GenerateD2Content(projectAliases, scope);

                    // Create the file and return the fully-qualified file path
                    return await CreateD2FileAsync(d2Content, docsPath, scope);
                })
                .ToListAsync();

            var extensions = new[] { "svg", "png" /*, "pdf"*/ };

            await d2FilePaths
                .SelectMany(path =>
                {
                    // Create all combinations of files to be created
                    return extensions.Select(extension => new
                    {
                        D2FilePath = path,
                        Extension = extension
                    });
                })
                .ForEachAsTaskAsync(async item =>
                {
                    // And process them in, at most, 8 tasks
                    await ExportD2ImageFileAsync(item.D2FilePath, item.Extension);
                }, 8);
        }

        private static ProjectAliases GetProjectAliases(string solutionPath, string projectsRootPath)
        {
            // The paths are required to work out dependency project absolute paths from their relative paths
            var solutionParser = new SolutionParser(solutionPath, projectsRootPath);

            var allAliases = new Dictionary<string, string>();              // e.g., alloverit-assertion: AllOverIt.Assertion
            var allDependencies = new HashSet<ProjectDependency>();

            foreach (var project in solutionParser.Projects)
            {
                var projectAlias = project.Name.Replace(".", "-").ToLowerInvariant();

                UpdateAliasCache(projectAlias, project.Name, allAliases);
                AddProjectDependencies(projectAlias, project.Dependencies, allAliases, allDependencies);
                AddPackageDependencies(projectAlias, project.Dependencies, allAliases, allDependencies);
            }

            return new ProjectAliases(allAliases, allDependencies);
        }

        private static void WriteDependenciesToConsole(ProjectAliases projectAliases)
        {
            var allAliases = projectAliases.AllAliases;
            var allDependencies = projectAliases.AllDependencies;

            var sortedDependenies = allDependencies
                .OrderBy(dependency => dependency.Alias)
                .ThenBy(dependency => dependency.DependencyAlias);

            foreach (var (alias, dependencyAlias) in sortedDependenies)
            {
                var aliasName = allAliases[alias];
                var dependencyName = allAliases[dependencyAlias];

                WriteDependencyToConsole(aliasName, dependencyName);
            }

            Console.WriteLine();
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
                allDependencies = allDependencies.Where(item => item.Alias == scope).AsReadOnlyCollection();

                aliasesUsed = new Dictionary<string, string>();

                foreach (var (alias, dependencyAlias) in allDependencies)
                {
                    UpdateAliasCache(alias, allAliases[alias], aliasesUsed);
                    UpdateAliasCache(dependencyAlias, allAliases[dependencyAlias], aliasesUsed);
                }
            }

            foreach (var alias in aliasesUsed)
            {
                // Alias: Display Name
                sb.AppendLine($"{GetDiagramAliasId(alias.Key)}: {alias.Value}");

                // Style non-AllOverIt dependencies
                if (!alias.Key.StartsWith("alloverit"))
                {
                    sb.AppendLine($"{alias.Key}.style.fill: lightblue");
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

        private static void WriteDependencyToConsole(string aliasName, string dependencyName)
        {
            var foreground = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{aliasName}");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" depends on ");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{dependencyName}");
            }
            finally
            {
                Console.ForegroundColor = foreground;
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

        private static void UpdateAliasCache(string alias, string name, IDictionary<string, string> aliases)
        {
            if (!aliases.ContainsKey(alias))
            {
                aliases.Add(alias, name);
            }
        }

        private static void AddProjectDependencies(string projectAlias, IReadOnlyCollection<ConditionalReferences> projectDependencies,
            IDictionary<string, string> allAliases, HashSet<ProjectDependency> allDependencies)
        {
            var allProjectDependencies = projectDependencies.SelectMany(dependency => dependency.ProjectReferences);

            foreach (var projectDependency in allProjectDependencies)
            {
                var projectDependencyName = Path.GetFileNameWithoutExtension(projectDependency.Path);

                var projectDependencyAlias = projectDependencyName.Replace(".", "-").ToLowerInvariant();

                UpdateAliasCache(projectDependencyAlias, projectDependencyName, allAliases);

                var dependency = new ProjectDependency(projectAlias, projectDependencyAlias);
                allDependencies.Add(dependency);
            }
        }

        private static void AddPackageDependencies(string projectAlias, IReadOnlyCollection<ConditionalReferences> projectDependencies,
            IDictionary<string, string> allAliases, HashSet<ProjectDependency> allDependencies/*, IDictionary<string, string> aliasGroups*/)
        {
            var allPackageDependencies = projectDependencies.SelectMany(dependency => dependency.PackageReferences);

            foreach (var packageDependency in allPackageDependencies)
            {
                var packageDependencyName = packageDependency.Name;

                var packageDependencyAlias = packageDependencyName.Replace(".", "-").ToLowerInvariant();

                UpdateAliasCache(packageDependencyAlias, packageDependencyName, allAliases);

                var dependency = new ProjectDependency(projectAlias, packageDependencyAlias);
                allDependencies.Add(dependency);
            }
        }
    }
}