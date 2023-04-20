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
        private const string PackageStyleFill = "#ADD8E6";
        private const string TransitiveStyleFill = "#FFEC96";
        private static readonly string[] ImageExtensions = new[] { "svg", "png" /*, "pdf"*/ };

        public static async Task CreateAllFilesAsync(string solutionPath, string projectsRootPath, string docsPath, Action<SolutionProject> reportDiagramData)
        {
            // The paths are required to work out dependency project absolute paths from their relative paths
            _ = solutionPath.WhenNotNullOrEmpty();
            _ = projectsRootPath.WhenNotNullOrEmpty();
            _ = docsPath.WhenNotNullOrEmpty();

            var solutionParser = new SolutionParser();
            var allProjects = await solutionParser.ParseAsync(solutionPath, projectsRootPath);

            foreach (var project in allProjects)
            {
                reportDiagramData.Invoke(project);
            }

            var indexedProjects = allProjects.ToDictionary(project => project.Name, project => project);

            await ExportAsIndividual(indexedProjects, docsPath);
            await ExportAsAll(indexedProjects, docsPath);
        }

        private static async Task ExportAsIndividual(IDictionary<string, SolutionProject> indexedProjects, string docsPath)
        {
            foreach (var scopedProject in indexedProjects.Values)
            {
                var d2Content = GenerateD2Content(scopedProject, indexedProjects);

                await CreateD2FileAndImages(scopedProject.Name, d2Content, docsPath);
            }
        }

        private static Task ExportAsAll(Dictionary<string, SolutionProject> indexedProjects, string docsPath)
        {
            var d2Content = GenerateD2Content(indexedProjects);

            return CreateD2FileAndImages("AllOverIt-All", d2Content, docsPath);
        }

        private static async Task CreateD2FileAndImages(string scope, string d2Content, string docsPath)
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
            AppendProjectDependencies(solutionProject, solutionProjects, dependencySet, 1);

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
                AppendProjectDependencies(solutionProject.Value, solutionProjects, dependencySet, 0);
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

        private static string GetDiagramAliasId(string alias, bool includeAoiPrefixIfApplicable = true)
        {
            alias = alias.ToLowerInvariant().Replace(".", "-");

            // Group all 'AllOverIt' packages together
            return includeAoiPrefixIfApplicable && alias.StartsWith("alloverit")
                ? $"aoi.{alias}"
                : alias;
        }
    }
}