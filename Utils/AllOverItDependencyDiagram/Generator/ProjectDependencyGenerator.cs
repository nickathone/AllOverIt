using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Logging;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using AllOverItDependencyDiagram.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AllOverItDependencyDiagram.Generator
{
    public sealed class ProjectDependencyGenerator
    {
        private readonly ProjectDependencyGeneratorOptions _options;
        private readonly IColorConsoleLogger _logger;

        private string _projectGroupName;
        private string _projectGroupPrefix;

        public ProjectDependencyGenerator(ProjectDependencyGeneratorOptions options, IColorConsoleLogger logger)
        {
            _options = options.WhenNotNull();
            _logger = logger.WhenNotNull();
        }

        public async Task CreateDiagramsAsync(string solutionPath, string projectsRootPath, string targetFramework)
        {
            // The paths are required to work out dependency project absolute paths from their relative paths.
            // projectsRootPath is the root of all projects to be processed (to ensure other sub-folders are excluded).
            _ = solutionPath.WhenNotNullOrEmpty();
            _ = projectsRootPath.WhenNotNullOrEmpty();

            InitProjectGroupInfo(solutionPath);

            var solutionParser = new SolutionParser(Math.Max(_options.IndividualProjectTransitiveDepth, _options.AllProjectsTransitiveDepth));
            var allProjects = await solutionParser.ParseAsync(solutionPath, projectsRootPath, targetFramework);

            foreach (var project in allProjects)
            {
                LogDependencies(project);
            }

            var solutionProjects = allProjects.ToDictionary(project => project.Name, project => project);

            await ExportAsSummary(solutionProjects);
            await ExportAsIndividual(solutionProjects);
            await ExportAsAll(solutionProjects);
        }

        private void InitProjectGroupInfo(string solutionPath)
        {
            _projectGroupName = Path.GetFileNameWithoutExtension(solutionPath);

            var regex = new Regex("[A-Z]");

            var matches = regex.Matches(_projectGroupName);

            // Cater for when the incoming solution name is all lowercase
            if (matches.Count == 0)
            {
                _projectGroupPrefix = _projectGroupName.ToLowerInvariant();
            }
            else
            {
                var capitalLetters = new char[matches.Count];
                var i = 0;

                foreach (var match in matches.Cast<Match>())
                {
                    capitalLetters[i++] = match.Value[0];
                }

                _projectGroupPrefix = new string(capitalLetters).ToLowerInvariant();
            }
        }



        /*
            55A9EE: A brighter blue that should provide good contrast against white and also complement the other colors you've chosen.
            6EBE50: A green color that is similar to 62B178 but brighter, and should provide good contrast against white.
            C56EE0: A purple color that is similar to B19CD9 but brighter, and should provide good contrast against white.
            FF8C67: An orange color that is similar to FF7F50 but brighter, and should provide good contrast against white.
            E3505C: A red color that is similar to CB484D but brighter, and should provide good contrast against white.
            FFC33C: A bright yellow color that can complement the other colors you've chosen and should provide good contrast against white.
         */

        private const string Blue = "55A9EE";
        private const string Green = "6EBE50";
        private const string Purple = "C56EE0";
        private const string Orange = "FF8C67";
        private const string Red = "E3505C";
        private const string Yellow = "FFC33C";

        public static readonly IDictionary<string, string> TargetFrameworkBadges = new Dictionary<string, string>
        {
            { "net7.0", $"![](https://img.shields.io/badge/.NET-7.0-{Blue}.svg)"},
            { "net7.0-windows", $"![](https://img.shields.io/badge/.NET-7.0--windows-{Blue}.svg)"},
            { "net6.0", $"![](https://img.shields.io/badge/.NET-6.0-{Orange}.svg)"},
            { "net6.0-windows", $"![](https://img.shields.io/badge/.NET-6.0--windows-{Orange}.svg)"},
            { "net5.0", $"![](https://img.shields.io/badge/.NET-5.0-{Yellow}.svg)"},
            { "net5.0-windows", $"![](https://img.shields.io/badge/.NET-5.0--windows-{Yellow}.svg)"},
            { "netcoreapp3.1", $"![](https://img.shields.io/badge/.NET-coreapp3.1-{Purple}.svg)"},
            { "netstandard2.1", $"![](https://img.shields.io/badge/.NET-standard2.1-{Green}.svg)"},
            { "netstandard2.0", $"![](https://img.shields.io/badge/.NET-standard2.0-{Red}.svg)"}            
        };

        private Task ExportAsSummary(IDictionary<string, SolutionProject> solutionProjects)
        {
            var sb = new StringBuilder();

            sb.AppendLine("# Dependency Summary");
            sb.AppendLine();

            var maxLengths = new int[3];

            foreach (var solutionProject in solutionProjects)
            {
                var project = Path.GetFileNameWithoutExtension(solutionProject.Value.Path);
                sb.AppendLine($"## {project}");
                sb.AppendLine();


                var frameworkBadges = TargetFrameworkBadges.Keys
                    .Intersect(solutionProject.Value.TargetFrameworks)
                    .Select(key => TargetFrameworkBadges[key])
                    .ToList();

                var projectBadges = string.Join(" ", frameworkBadges);
                sb.AppendLine(projectBadges);

                sb.AppendLine();

                sb.AppendLine("|Dependencies|");
                sb.AppendLine("|:-|");


                var dependencySet = new HashSet<string>();
                var transitiveSet = new HashSet<string>();

                void AppendProjectDependencies(SolutionProject solutionProject)
                {
                    AppendPackageDependencies(solutionProject);

                    foreach (var project in solutionProject.Dependencies.SelectMany(item => item.ProjectReferences))
                    {
                        AppendProjectDependenciesRecursively(project);
                    }
                }

                void AppendProjectDependenciesRecursively(ProjectReference projectReference)
                {
                    var projectName = GetProjectName(projectReference);

                    dependencySet.Add(projectName);

                    // Add all packages dependencies (recursively) for the current project
                    foreach (var package in solutionProjects[projectName].Dependencies.SelectMany(item => item.PackageReferences))
                    {
                        AppendPackageDependenciesRecursively(package);
                    }

                    // Add all project dependencies (recursively) for the current project
                    foreach (var project in solutionProjects[projectName].Dependencies.SelectMany(item => item.ProjectReferences))
                    {
                        AppendProjectDependenciesRecursively(project);
                    }
                }

                void AppendPackageDependencies(SolutionProject solutionProject)
                {
                    var projectName = solutionProject.Name;

                    foreach (var package in solutionProject.Dependencies.SelectMany(item => item.PackageReferences))
                    {
                        AppendPackageDependenciesRecursively(package);
                    }
                }

                void AppendPackageDependenciesRecursively(PackageReference packageReference)
                {
                    var packageNameVersion = $"{packageReference.Name} v{packageReference.Version}";

                    if (packageReference.Depth == 0)
                    {
                        dependencySet.Add(packageNameVersion);
                    }
                    else
                    {
                        transitiveSet.Add(packageNameVersion);
                    }

                    foreach (var package in packageReference.TransitiveReferences)
                    {
                        AppendPackageDependenciesRecursively(package);
                    }
                }

                AppendProjectDependencies(solutionProject.Value);


                var dependencies = dependencySet.Concat(transitiveSet).Order().ToArray();

                if (dependencies.Any())
                {
                    foreach (var dependency in dependencies)
                    {
                        sb.AppendLine($"|{dependency}|");
                    }
                }
                else
                {
                    sb.AppendLine("None");
                }

                sb.AppendLine();
                sb.AppendLine($"<br>");
                sb.AppendLine();
                sb.AppendLine($"---");
                sb.AppendLine();
                sb.AppendLine($"<br>");
                sb.AppendLine();
            }

            var content = sb.ToString();

            var summaryPath = Path.Combine(_options.DiagramExportPath, "summary.md");

            return File.WriteAllTextAsync(summaryPath, content);
        }

        private async Task ExportAsIndividual(IDictionary<string, SolutionProject> solutionProjects)
        {
            foreach (var scopedProject in solutionProjects.Values)
            {
                var d2Content = GenerateD2Content(scopedProject, solutionProjects);

                await CreateD2FileAndImages(scopedProject.Name, d2Content);
            }
        }

        private Task ExportAsAll(IDictionary<string, SolutionProject> solutionProjects)
        {
            var d2Content = GenerateD2Content(solutionProjects);

            return CreateD2FileAndImages("AllOverIt-All", d2Content);
        }

        private async Task CreateD2FileAndImages(string projectScope, string d2Content)
        {
            // Create the file and return the fully-qualified file path
            var filePath = await CreateD2FileAsync(d2Content, GetDiagramAliasId(projectScope, false));

            foreach (var format in _options.ImageFormats)
            {
                await ExportD2ImageFileAsync(filePath, format);
            }
        }

        private string GenerateD2Content(SolutionProject solutionProject, IDictionary<string, SolutionProject> solutionProjects)
        {
            var sb = new StringBuilder();

            sb.AppendLine("direction: right");
            sb.AppendLine();

            sb.AppendLine($"aoi: AllOverIt");

            var dependencySet = new HashSet<string>();
            AppendProjectDependencies(solutionProject, solutionProjects, dependencySet, _options.IndividualProjectTransitiveDepth);

            foreach (var dependency in dependencySet)
            {
                sb.AppendLine(dependency);
            }

            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateD2Content(IDictionary<string, SolutionProject> solutionProjects)
        {
            var sb = new StringBuilder();

            sb.AppendLine("direction: right");
            sb.AppendLine();

            sb.AppendLine($"{_projectGroupPrefix}: {_projectGroupName}");

            var dependencySet = new HashSet<string>();

            foreach (var solutionProject in solutionProjects)
            {
                AppendProjectDependencies(solutionProject.Value, solutionProjects, dependencySet, _options.AllProjectsTransitiveDepth);
            }

            foreach (var dependency in dependencySet)
            {
                sb.AppendLine(dependency);
            }

            sb.AppendLine();

            return sb.ToString();
        }

        private void AppendProjectDependencies(SolutionProject solutionProject, IDictionary<string, SolutionProject> solutionProjects,
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

        private void AppendProjectDependenciesRecursively(ProjectReference projectReference, IDictionary<string, SolutionProject> solutionProjects,
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

        private void AppendPackageDependencies(SolutionProject solutionProject, HashSet<string> dependencySet, int maxTransitiveDepth)
        {
            var projectName = solutionProject.Name;
            var projectAlias = GetDiagramAliasId(projectName);

            foreach (var package in solutionProject.Dependencies.SelectMany(item => item.PackageReferences))
            {
                var added = AppendPackageDependenciesRecursively(package, dependencySet, maxTransitiveDepth);

                if (added)
                {
                    dependencySet.Add($"{GetPackageAliasId(package)} <- {projectAlias}");
                }
            }
        }

        private bool AppendPackageDependenciesRecursively(PackageReference packageReference, HashSet<string> dependencySet, int maxTransitiveDepth)
        {
            if (packageReference.Depth > maxTransitiveDepth)
            {
                return false;
            }

            var packageName = packageReference.Name;
            var packageAlias = GetDiagramPackageAliasId(packageReference);

            dependencySet.Add($"{packageAlias}: {packageName}\\nv{packageReference.Version}");

            var transitiveStyleFillEntry = GetTransitiveStyleFillEntry(packageAlias);
            var packageStyleFillEntry = GetPackageStyleFillEntry(packageAlias);

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

        private string GetPackageStyleFillEntry(string packageAlias)
        {
            return $"{packageAlias}.style.fill: \"{_options.PackageStyleFill}\"";
        }

        private string GetTransitiveStyleFillEntry(string packageAlias)
        {
            return $"{packageAlias}.style.fill: \"{_options.TransitiveStyleFill}\"";
        }

        private static string GetProjectName(ProjectReference projectReference)
        {
            return Path.GetFileNameWithoutExtension(projectReference.Path);
        }

        private string GetProjectAliasId(ProjectReference projectReference)
        {
            return GetDiagramAliasId(GetProjectName(projectReference));
        }

        private static string GetPackageAliasId(PackageReference packageReference)
        {
            return GetDiagramPackageAliasId(packageReference);
        }

        private async Task<string> CreateD2FileAsync(string content, string projectScope)
        {
            var fileName = projectScope.IsNullOrEmpty()
                ? $"{_projectGroupName.ToLowerInvariant()}-all.d2"
                : $"{projectScope}.d2";

            var d2FilePath = Path.Combine(_options.DiagramExportPath, fileName);

            // Showing how to mix AddFormatted() with AddFragment() where the latter
            // is a simple alternative to using string interpolation.
            _logger.Write("{forecolor:white}Creating diagram: ")
                   .Write(ConsoleColor.Yellow, Path.GetFileName(fileName))
                   .Write("{forecolor:white}...");

            File.WriteAllText(d2FilePath, content);

            await ProcessBuilder
                .For("d2.exe")
                .WithArguments("fmt", d2FilePath)
                .BuildProcessExecutor()
                .ExecuteAsync();

            // An example using formatted text
            _logger.WriteLine("{forecolor:green}Done");

            return d2FilePath;
        }

        private async Task ExportD2ImageFileAsync(string d2FileName, DiagramImageFormat format)
        {
            var imageFileName = Path.ChangeExtension(d2FileName, $"{format}").ToLowerInvariant();

            _logger
                .Write(ConsoleColor.White, "Creating image: ")
                .Write(ConsoleColor.Yellow, Path.GetFileName(imageFileName))
                .Write(ConsoleColor.White, "...");

            var export = ProcessBuilder
               .For("d2.exe")
               .WithArguments("-l", "elk", d2FileName, imageFileName)
               .BuildProcessExecutor();

            await export.ExecuteAsync();

            // An example using a foreground color and text
            _logger.WriteLine(ConsoleColor.Green, "Done");
        }

        private string GetDiagramAliasId(string alias, bool includeProjectGroupPrefix = true)
        {
            alias = alias.ToLowerInvariant().Replace(".", "-");

            return includeProjectGroupPrefix
                ? $"{_projectGroupPrefix}.{alias}"
                : alias;
        }

        private static string GetDiagramPackageAliasId(PackageReference package)
        {
            return $"{package.Name}_{package.Version}".Replace(".", "-").ToLowerInvariant();
        }

        private void LogDependencies(SolutionProject solutionProject)
        {
            var sortedProjectDependenies = solutionProject.Dependencies
                .SelectMany(item => item.ProjectReferences)
                .Select(item => item.Path)
                .Order();

            foreach (var dependency in sortedProjectDependenies)
            {
                _logger
                    .Write(ConsoleColor.Yellow, solutionProject.Name)
                    .Write(ConsoleColor.White, " depends on ")
                    .WriteLine(ConsoleColor.Yellow, Path.GetFileNameWithoutExtension(dependency));
            }

            var sortedPackageDependenies = solutionProject.Dependencies
                .SelectMany(item => GetAllPackageDependencies(item.PackageReferences))
                .Select(item => (item.Name, item.Version))
                .Distinct()                                     // Multiple packages may depend on another common package
                .Order()
                .GroupBy(item => item.Name);

            foreach (var dependency in sortedPackageDependenies)
            {
                var dependencyName = dependency.Key;
                var dependencyVersions = dependency.ToList();

                if (dependencyVersions.Count == 1)
                {
                    var dependencyVersion = dependencyVersions.Single();

                    _logger
                        .Write(ConsoleColor.Yellow, solutionProject.Name)
                        .Write(ConsoleColor.White, " depends on ")
                        .WriteLine(ConsoleColor.Yellow, $"{dependencyName} v{dependencyVersion.Version}");
                }
                else
                {
                    var versions = dependencyVersions.Select(item => $"v{item.Version}");

                    _logger
                        .WriteLine(ConsoleColor.Red, $"{solutionProject.Name} depends on multiple versions of {dependencyName} {string.Join(", ", versions)}");
                }
            }
        }

        private static IEnumerable<PackageReference> GetAllPackageDependencies(IEnumerable<PackageReference> packageReferences)
        {
            foreach (var packageReference in packageReferences)
            {
                yield return packageReference;

                foreach (var transitiveReference in GetAllPackageDependencies(packageReference.TransitiveReferences))
                {
                    yield return transitiveReference;
                }
            }
        }
    }
}