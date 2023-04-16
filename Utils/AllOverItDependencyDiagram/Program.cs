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

    internal class Program
    {
        static async Task Main(/*string[] args*/)
        {
            var applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var allOverItRoot = FileUtils.GetAbsolutePath(applicationPath, @"..\..\..\..\..\");
            var solutionPath = Path.Combine(allOverItRoot, "AllOverIt.sln");
            var projectsRootPath = Path.Combine(allOverItRoot, "Source");

            // The paths are required to work out depencdeny project absolute paths from their relative paths
            var solutionParser = new SolutionParser(solutionPath, new[] { projectsRootPath });

            var allAliases = new Dictionary<string, string>();              // e.g., alloverit-assertion: AllOverIt.Assertion
            var allDependencies = new HashSet<ProjectDependency>();

            foreach (var project in solutionParser.Projects)
            {
                var projectAlias = project.Name.Replace(".", "-").ToLowerInvariant();

                UpdateAliasCache(projectAlias, project.Name, allAliases);
                AddProjectDependencies(projectAlias, project.Dependencies, allAliases, allDependencies);
                AddPackageDependencies(projectAlias, project.Dependencies, allAliases, allDependencies/*, aliasGroups*/);
            }

            var sortedDependenies = allDependencies
                .OrderBy(dependency => dependency.Alias)
                .ThenBy(dependency => dependency.DependencyAlias);

            foreach (var (alias, dependencyAlias) in sortedDependenies)
            {
                var aliasName = allAliases[alias];
                var dependencyName = allAliases[dependencyAlias];

                WriteDependsOnToConsole(aliasName, dependencyName);
            }

            Console.WriteLine();

            var d2Content = GenerateD2Content(allAliases, sortedDependenies);

            var docsPath = Path.Combine(allOverItRoot, @"Docs\Dependencies");
            await CreateD2FilesAsync(d2Content, docsPath);

            Console.WriteLine();
            Console.WriteLine($"AllOverIt.");
        }

        private static string GenerateD2Content(IDictionary<string, string> allAliases, IOrderedEnumerable<ProjectDependency> sortedDependenies)
        {
            var sb = new StringBuilder();

            sb.AppendLine("direction: right");
            sb.AppendLine();

            sb.AppendLine($"aoi: AllOverIt");

            foreach (var alias in allAliases)
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

            foreach (var (alias, dependencyAlias) in sortedDependenies)
            {
                sb.AppendLine($"{GetDiagramAliasId(dependencyAlias)} <- {GetDiagramAliasId(alias)}");
            }

            return sb.ToString();
        }

        private static void WriteDependsOnToConsole(string aliasName, string dependencyName)
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

        private static async Task CreateD2FilesAsync(string content, string docsPath)
        {
            var d2FileName = Path.Combine(docsPath, "AllOverIt.d2");

            Console.WriteLine($"Creating D2 diagram...");

            await CreateD2DiagramAsync(d2FileName, content);

            Console.WriteLine($"Exporting images...");

            await Task.WhenAll(
                ExportD2ImageFileAsync(d2FileName, "svg"),
                ExportD2ImageFileAsync(d2FileName, "png"),
                ExportD2ImageFileAsync(d2FileName, "pdf")
            );
        }

        private static async Task CreateD2DiagramAsync(string d2FileName, string content)
        {
            File.WriteAllText(d2FileName, content);

            await ProcessBuilder
                .For("d2.exe")
                .WithArguments("fmt", d2FileName)
                .BuildProcessExecutor()
                .ExecuteAsync();
        }

        private static async Task ExportD2ImageFileAsync(string d2FileName, string extension)
        {
            var imageFileName = Path.ChangeExtension(d2FileName, extension);

            var export = ProcessBuilder
               .For("d2.exe")
               .WithArguments("-l", "elk", d2FileName, imageFileName)
               .BuildProcessExecutor();

            await export.ExecuteAsync();

            Console.WriteLine($"Exported image as {imageFileName}");
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