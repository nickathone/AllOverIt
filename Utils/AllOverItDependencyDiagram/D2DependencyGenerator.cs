using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using AllOverItDependencyDiagram.Logging;
using SolutionInspector.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionInspector
{
    internal sealed class D2DependencyGenerator
    {
        // These could all become options if required
        private const int IndividualProjectTransitiveDepth = 1;
        private const int AllProjectsTransitiveDepth = 0;
        private const string PackageStyleFill = "#ADD8E6";
        private const string TransitiveStyleFill = "#FFEC96";
        private static readonly string[] ImageExtensions = new[] { "svg", "png" /*, "pdf"*/ };

        private readonly IConsoleLogger _logger;

        public D2DependencyGenerator(IConsoleLogger logger)
        {
            _logger = logger.WhenNotNull();
        }

        public async Task CreateDiagramsAsync(string solutionPath, string projectsRootPath, string docsPath)
        {
            // The paths are required to work out dependency project absolute paths from their relative paths
            _ = solutionPath.WhenNotNullOrEmpty();
            _ = projectsRootPath.WhenNotNullOrEmpty();
            _ = docsPath.WhenNotNullOrEmpty();

            var solutionParser = new SolutionParser(Math.Max(IndividualProjectTransitiveDepth, AllProjectsTransitiveDepth));
            var allProjects = await solutionParser.ParseAsync(solutionPath, projectsRootPath);

            foreach (var project in allProjects)
            {
                LogDependenciesToConsole(project);
            }

            var indexedProjects = allProjects.ToDictionary(project => project.Name, project => project);

            await ExportAsIndividual(indexedProjects, docsPath);
            await ExportAsAll(indexedProjects, docsPath);
        }   

        private async Task ExportAsIndividual(IDictionary<string, SolutionProject> indexedProjects, string docsPath)
        {
            foreach (var scopedProject in indexedProjects.Values)
            {
                var d2Content = GenerateD2Content(scopedProject, indexedProjects);

                await CreateD2FileAndImages(scopedProject.Name, d2Content, docsPath);
            }
        }

        private Task ExportAsAll(IDictionary<string, SolutionProject> indexedProjects, string docsPath)
        {
            var d2Content = GenerateD2Content(indexedProjects);

            return CreateD2FileAndImages("AllOverIt-All", d2Content, docsPath);
        }

        private async Task CreateD2FileAndImages(string scope, string d2Content, string docsPath)
        {
            // Create the file and return the fully-qualified file path
            var filePath = await CreateD2FileAsync(d2Content, docsPath, GetDiagramAliasId(scope, false));

            foreach (var extension in ImageExtensions)
            {
                await ExportD2ImageFileAsync(filePath, extension);
            }
        }

        private static string GenerateD2Content(SolutionProject solutionProject, IDictionary<string, SolutionProject> solutionProjects)
        {
            var sb = new StringBuilder();

            sb.AppendLine("direction: right");
            sb.AppendLine();

            sb.AppendLine($"aoi: AllOverIt");

            var dependencySet = new HashSet<string>();
            AppendProjectDependencies(solutionProject, solutionProjects, dependencySet, IndividualProjectTransitiveDepth);

            foreach (var dependency in dependencySet)
            {
                sb.AppendLine(dependency);
            }

            sb.AppendLine();

            return sb.ToString();
        }

        private static string GenerateD2Content(IDictionary<string, SolutionProject> solutionProjects)
        {
            var sb = new StringBuilder();

            sb.AppendLine("direction: right");
            sb.AppendLine();

            sb.AppendLine($"aoi: AllOverIt");

            var dependencySet = new HashSet<string>();

            foreach (var solutionProject in solutionProjects)
            {
                AppendProjectDependencies(solutionProject.Value, solutionProjects, dependencySet, AllProjectsTransitiveDepth);
            }

            foreach (var dependency in dependencySet)
            {
                sb.AppendLine(dependency);
            }

            sb.AppendLine();

            return sb.ToString();
        }

        private static void AppendProjectDependencies(SolutionProject solutionProject, IDictionary<string, SolutionProject> solutionProjects,
            HashSet<string> dependencySet, int maxTransitiveDepth)
        {

            var projectName = solutionProject.Name;
            var projectAlias = GetDiagramAliasId(projectName);

            dependencySet.Add($"{projectAlias}: {projectName}");

            AppendPackageDependencies(solutionProject, dependencySet, maxTransitiveDepth);

            foreach (var project in solutionProject.Dependencies.SelectMany(item => item.ProjectReferences))
            {
                AppendProjectDependenciesRecursively(project, solutionProjects, dependencySet, maxTransitiveDepth);

                dependencySet.Add($"{GetProjectAliasId(project)} <- {projectAlias}");
            }
        }

        private static void AppendProjectDependenciesRecursively(ProjectReference projectReference, IDictionary<string, SolutionProject> solutionProjects,
            HashSet<string> dependencySet, int maxTransitiveDepth)
        {
            var projectName = GetProjectName(projectReference);
            var projectAlias = GetDiagramAliasId(projectName);

            dependencySet.Add($"{projectAlias}: {projectName}");

            // Add all packages dependencies (recursively) for the current project
            foreach (var package in solutionProjects[projectName].Dependencies.SelectMany(item => item.PackageReferences))
            {
                var added = AppendPackageDependenciesRecursively(package, dependencySet, maxTransitiveDepth);

                if (added)
                {
                    dependencySet.Add($"{GetPackageAliasId(package)} <- {projectAlias}");
                }
            }

            // Add all project dependencies (recursively) for the current project
            foreach (var project in solutionProjects[projectName].Dependencies.SelectMany(item => item.ProjectReferences))
            {
                AppendProjectDependenciesRecursively(project, solutionProjects, dependencySet, maxTransitiveDepth);

                dependencySet.Add($"{GetProjectAliasId(project)} <- {projectAlias}");
            }
        }

        private static void AppendPackageDependencies(SolutionProject solutionProject, HashSet<string> dependencySet, int maxTransitiveDepth)
        {
            var projectName = solutionProject.Name;
            var projectAlias = GetDiagramAliasId(projectName);

            dependencySet.Add($"{projectAlias}: {projectName}");

            foreach (var package in solutionProject.Dependencies.SelectMany(item => item.PackageReferences))
            {
                var added = AppendPackageDependencies(package, dependencySet, maxTransitiveDepth);

                if (added)
                {
                    dependencySet.Add($"{GetPackageAliasId(package)} <- {projectAlias}");
                }
            }
        }

        private static bool AppendPackageDependencies(PackageReference packageReference, HashSet<string> dependencySet, int maxTransitiveDepth)
        {
            if (packageReference.Depth > maxTransitiveDepth)
            {
                return false;
            }

            var packageName = packageReference.Name;
            var packageAlias = GetDiagramAliasId(packageName);

            dependencySet.Add($"{packageAlias}: {packageName}");

            AppendPackageDependenciesRecursively(packageReference, dependencySet, maxTransitiveDepth);

            return true;
        }

        private static bool AppendPackageDependenciesRecursively(PackageReference packageReference, HashSet<string> dependencySet, int maxTransitiveDepth)
        {
            if (packageReference.Depth > maxTransitiveDepth)
            {
                return false;
            }

            var packageName = packageReference.Name;
            var packageAlias = GetDiagramAliasId(packageName);

            dependencySet.Add($"{packageAlias}: {packageName}");

            var transitiveStyleFillEntry = GetTransitiveStyleFillEntry(packageAlias);
            var packageStyleFillEntry = GetPackageStyleFillEntry(packageAlias); ;

            // The diagram should style package reference over transient reference
            if (packageReference.IsTransitive)
            {
                if (!dependencySet.Contains(packageStyleFillEntry))
                {
                    dependencySet.Add(transitiveStyleFillEntry);
                }
            }
            else
            {
                dependencySet.Remove(transitiveStyleFillEntry);
                dependencySet.Add(packageStyleFillEntry);
            }

            dependencySet.Add($"{packageAlias}.style.opacity: 0.8");

            foreach (var package in packageReference.TransitiveReferences)
            {
                var added = AppendPackageDependenciesRecursively(package, dependencySet, maxTransitiveDepth);

                if (added)
                {
                    dependencySet.Add($"{GetPackageAliasId(package)} <- {packageAlias}");
                }
            }

            return true;
        }

        private static string GetPackageStyleFillEntry(string packageAlias)
        {
            return $"{packageAlias}.style.fill: \"{PackageStyleFill}\"";
        }

        private static string GetTransitiveStyleFillEntry(string packageAlias)
        {
            return $"{packageAlias}.style.fill: \"{TransitiveStyleFill}\"";
        }

        private static string GetProjectName(ProjectReference project)
        {
            return Path.GetFileNameWithoutExtension(project.Path);
        }

        private static string GetProjectAliasId(ProjectReference project)
        {
            return GetDiagramAliasId(GetProjectName(project));
        }

        private static string GetPackageAliasId(PackageReference package)
        {
            return GetDiagramAliasId(package.Name);
        }

        private async Task<string> CreateD2FileAsync(string content, string docsPath, string scope)
        {
            var fileName = scope.IsNullOrEmpty()
                ? "alloverit-all.d2"
                : $"{scope}.d2";

            var d2FilePath = Path.Combine(docsPath, fileName);

            // Showing how to mix AddFormatted() with AddFragment() where the latter
            // is a simple alternative to using string interpolation.
            _logger.WriteFormatted("{forecolor:white}Creating D2 diagram for ")
                   .WriteFragment(ConsoleColor.Yellow, Path.GetFileNameWithoutExtension(fileName))
                   .WriteFormatted("{forecolor:white}...");

            File.WriteAllText(d2FilePath, content);

            await ProcessBuilder
                .For("d2.exe")
                .WithArguments("fmt", d2FilePath)
                .BuildProcessExecutor()
                .ExecuteAsync();

            // An example using formatted text
            _logger.WriteFormattedLine("{forecolor:green}Done");

            return d2FilePath;
        }

        private async Task ExportD2ImageFileAsync(string d2FileName, string extension)
        {
            var imageFileName = Path.ChangeExtension(d2FileName, extension);

            _logger
                .WriteFragment(ConsoleColor.White, "Creating image ")
                .WriteFragment(ConsoleColor.Yellow, imageFileName)
                .WriteFragment(ConsoleColor.White, "...");

            var export = ProcessBuilder
               .For("d2.exe")
               .WithArguments("-l", "elk", d2FileName, imageFileName)
               .BuildProcessExecutor();

            await export.ExecuteAsync();

            // An example using a foreground color and text
            _logger.WriteLine(ConsoleColor.Green, "Done");
        }

        private static string GetDiagramAliasId(string alias, bool includeAoiPrefixIfApplicable = true)
        {
            alias = alias.ToLowerInvariant().Replace(".", "-");

            // Group all 'AllOverIt' packages together
            return includeAoiPrefixIfApplicable && alias.StartsWith("alloverit")
                ? $"aoi.{alias}"
                : alias;
        }

        private void LogDependenciesToConsole(SolutionProject solutionProject)
        {
            var sortedProjectDependenies = solutionProject.Dependencies
                .SelectMany(item => item.ProjectReferences)
                .Select(item => item.Path)
                .Order();

            foreach (var dependency in sortedProjectDependenies)
            {
                _logger
                    .WriteFragment(ConsoleColor.Yellow, solutionProject.Name)
                    .WriteFragment(ConsoleColor.White, " depends on ")
                    .WriteLine(ConsoleColor.Yellow, Path.GetFileNameWithoutExtension(dependency));
            }

            var sortedPackageDependenies = solutionProject.Dependencies
                .SelectMany(item => item.PackageReferences)
                .Select(item => item.Name)
                .Distinct()                                     // Multiple packages may depend on another common package
                .Order().ToList();

            foreach (var dependency in sortedPackageDependenies)
            {
                _logger
                    .WriteFragment(ConsoleColor.Yellow, solutionProject.Name)
                    .WriteFragment(ConsoleColor.White, " depends on ")
                    .WriteLine(ConsoleColor.Yellow, dependency);
            }
        }
    }    
}