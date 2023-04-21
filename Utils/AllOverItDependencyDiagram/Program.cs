using AllOverIt.Io;
using AllOverItDependencyDiagram.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SolutionInspector
{
    // The current version is not using DI so unit testability hasn't been a priority
    internal class Program
    {
        static async Task Main()
        {
            var applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var allOverItRoot = FileUtils.GetAbsolutePath(applicationPath, @"..\..\..\..\..\");
            var solutionPath = Path.Combine(allOverItRoot, "AllOverIt.sln");
            var projectsRootPath = Path.Combine(allOverItRoot, "Source");

            var docsPath = Path.Combine(allOverItRoot, @"Docs\Dependencies");

            var logger = new DependencyGeneratorLogger();
            var generator = new D2DependencyGenerator(logger);

            await generator.CreateDiagramsAsync(solutionPath, projectsRootPath, docsPath);

            Console.WriteLine();
            Console.WriteLine($"AllOverIt.");
        }
    }
}