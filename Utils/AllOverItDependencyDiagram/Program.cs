using AllOverIt.Io;
using AllOverIt.Logging;
using AllOverItDependencyDiagram.Generator;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AllOverItDependencyDiagram
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

            var options = new ProjectDependencyGeneratorOptions
            {
                ExportPath = Path.Combine(allOverItRoot, @"Docs\Dependencies")
            };

            // Uncomment this line to generate the D2 files but not the images
            // options.ImageFormats.Clear();

            var logger = new ColorConsoleLogger();
            var generator = new ProjectDependencyGenerator(options, logger);

            await generator.CreateDiagramsAsync(solutionPath, projectsRootPath, "net7.0");

            Console.WriteLine();
            Console.WriteLine($"AllOverIt.");
        }
    }
}