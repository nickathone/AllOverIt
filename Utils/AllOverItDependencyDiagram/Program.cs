using AllOverIt.Io;
using SolutionInspector.Parser;
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

        private static void WriteDependenciesToConsole(SolutionProject solutionProject)
        {
            var sortedDependenies = solutionProject.Dependencies
                .SelectMany(item => item.ProjectReferences)
                .Select(item => item.Path)
                .Order();

            foreach (var dependency in sortedDependenies)
            {
                WriteDependencyToConsole(solutionProject.Name, Path.GetFileNameWithoutExtension(dependency));
            }
        }

        private static void WriteDependencyToConsole(string alias, string dependency)
        {
            var foreground = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{alias}");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" depends on ");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{dependency}");
            }
            finally
            {
                Console.ForegroundColor = foreground;
            }
        }
    }
}