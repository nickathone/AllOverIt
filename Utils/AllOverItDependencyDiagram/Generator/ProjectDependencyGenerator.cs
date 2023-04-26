using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using AllOverIt.Logging;
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

        private async Task ExportAsSummary(IDictionary<string, SolutionProject> solutionProjects)
        {
            var sb = new StringBuilder();

            sb.AppendLine("# Dependency Summary");

            var maxLengths = new int[3];

            foreach (var solutionProject in solutionProjects)
            {
                sb.AppendLine();
                sb.AppendLine("|Package|Dependency|Transitive|");
                sb.AppendLine("|-|-|-|");

                var dependencySet = new HashSet<string>();
                var transitiveSet = new HashSet<string>();



                void AppendProjectDependencies(SolutionProject solutionProject)
                {
                    //var projectName = solutionProject.Name;                  

                    //dependencySet.Add(projectName);

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




                // Projects and explicit packages
                AppendProjectDependencies(solutionProject.Value);




                var dependencies = dependencySet.Order().ToArray();
                var transitives = transitiveSet.Order().ToArray();

                maxLengths[0] = Math.Max(maxLengths[0], solutionProject.Value.Name.Length);

                if (dependencies.Any())
                {
                    maxLengths[1] = Math.Max(maxLengths[1], dependencies.Max(item => item.Length));
                }

                if (transitives.Any())
                {
                    maxLengths[2] = Math.Max(maxLengths[2], transitives.Max(item => item.Length));
                }


                static string GetElement<T>(T[] elements, int index, Func<T, string> selector)
                {
                    if (index >= elements.Length)
                    {
                        return string.Empty;
                    }

                    return selector.Invoke(elements[index]);
                }


                string GetSummaryLine(int index)
                {
                    var project = GetElement(new[] { solutionProject.Value }, index, item => Path.GetFileNameWithoutExtension(item.Path));

                    var package = GetElement(dependencies, index, item => item);

                    if (index == 0 && package.IsNullOrEmpty())
                    {
                        package = "-";
                    }

                    var transitive = GetElement(transitives, index, item => item);

                    return $"|{project}|{package}|{transitive}|";
                }

                var maxCount = Math.Max(dependencies.Length, transitives.Length);

                // Cater for when there's no dependencies - we at least want the project listed
                maxCount = Math.Max(maxCount, 1);

                for (var i = 0; i < maxCount; i++)
                {
                    var line = GetSummaryLine(i);
                    sb.AppendLine(line);
                }
            }



            var style = $$"""
                <style>
                  th:nth-child(1) {
                    width: {{maxLengths[0]}}ch;
                  }
                  th:nth-child(2) {
                    width: {{maxLengths[1]}}ch;
                  }
                  th:nth-child(3) {
                    width: {{maxLengths[2]}}ch;
                  }
                  tr:first-child th {
                    background-color: #444654;
                  }
                </style>


                """;        // only 1 newline is added to the content

            sb.Insert(0, style);

            var output = sb.ToString();
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
            _logger.WriteFormatted("{forecolor:white}Creating diagram: ")
                   .WriteFragment(ConsoleColor.Yellow, Path.GetFileName(fileName))
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

        private async Task ExportD2ImageFileAsync(string d2FileName, DiagramImageFormat format)
        {
            var imageFileName = Path.ChangeExtension(d2FileName, $"{format}").ToLowerInvariant();

            _logger
                .WriteFragment(ConsoleColor.White, "Creating image: ")
                .WriteFragment(ConsoleColor.Yellow, Path.GetFileName(imageFileName))
                .WriteFragment(ConsoleColor.White, "...");

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
                    .WriteFragment(ConsoleColor.Yellow, solutionProject.Name)
                    .WriteFragment(ConsoleColor.White, " depends on ")
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
                        .WriteFragment(ConsoleColor.Yellow, solutionProject.Name)
                        .WriteFragment(ConsoleColor.White, " depends on ")
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