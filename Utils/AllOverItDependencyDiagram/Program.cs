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

            var generator = new D2DependencyGenerator(solutionPath, projectsRootPath);

            WriteDependenciesToConsole(generator.ProjectAliases);

            var docsPath = Path.Combine(allOverItRoot, @"Docs\Dependencies");
            await generator.CreateAllFilesAsync(docsPath);

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
                var aliasName = allAliases[alias];
                var dependencyName = allAliases[dependencyAlias];

                WriteDependencyToConsole(aliasName, dependencyName);
            }

            Console.WriteLine();
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
    }
}