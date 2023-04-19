using AllOverIt.Io;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SolutionInspector
{
    internal class Program
    {
        static async Task Main()
        {
            var applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var allOverItRoot = FileUtils.GetAbsolutePath(applicationPath, @"..\..\..\..\..\");
            var solutionPath = Path.Combine(allOverItRoot, "AllOverIt.sln");
            var projectsRootPath = Path.Combine(allOverItRoot, "Source");

            var docsPath = Path.Combine(allOverItRoot, @"Docs\Dependencies");
            await D2DependencyGenerator.CreateAllFilesAsync(solutionPath, projectsRootPath, docsPath, WriteDependenciesToConsole);

            Console.WriteLine();
            Console.WriteLine($"AllOverIt.");
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
                WriteDependencyToConsole(allAliases[alias], allAliases[dependencyAlias]);
            }

            Console.WriteLine();
        }

        private static void WriteDependencyToConsole(ProjectAlias alias, ProjectAlias dependency)
        {
            var foreground = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{alias.DisplayName}");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" depends on ");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{dependency.DisplayName}");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" ({dependency.AliasType})");
            }
            finally
            {
                Console.ForegroundColor = foreground;
            }
        }
    }
}