using AllOverIt.Io;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionInspector.Parser
{
    internal sealed class SolutionParser
    {
        public IReadOnlyCollection<SolutionProject> Projects { get; }

        public SolutionParser(string solutionFilePath, string projectIncludePath)
        {
            Projects = GetProjects(solutionFilePath, projectIncludePath);
        }

        private static IReadOnlyCollection<SolutionProject> GetProjects(string solutionFilePath, string projectIncludePath)
        {
            var projects = new List<SolutionProject>();

            var solutionFile = SolutionFile.Parse(solutionFilePath);

            var orderedProjects = solutionFile.ProjectsInOrder
                .Where(project => project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
                .Where(project =>
                {
                    return project.AbsolutePath.StartsWith(projectIncludePath, StringComparison.OrdinalIgnoreCase);
                });

            foreach (var projectItem in orderedProjects)
            {
                var projectRootElement = ProjectRootElement.Open(projectItem.AbsolutePath);
                var projectFolder = Path.GetDirectoryName(projectItem.AbsolutePath);

                var targetFrameworks = GetTargetFrameworks(projectRootElement.PropertyGroups);
                var conditionalReferences = GetConditionalReferences(projectFolder, projectRootElement.ItemGroups).ToList();

                var project = new SolutionProject
                {
                    Name = projectItem.ProjectName,
                    Path = projectItem.AbsolutePath,
                    TargetFrameworks = targetFrameworks,
                    Dependencies = conditionalReferences
                };

                projects.Add(project);
            }

            return projects;
        }

        private static IReadOnlyCollection<string> GetTargetFrameworks(IEnumerable<ProjectPropertyGroupElement> propertyGroups)
        {
            return propertyGroups
                .SelectMany(grp => grp.Properties)
                .Where(prop => prop.Name.Equals("TargetFrameworks", StringComparison.OrdinalIgnoreCase) ||
                               prop.Name.Equals("TargetFramework", StringComparison.OrdinalIgnoreCase))
                .Select(prop => prop.Value)
                .Single()
                .Split(";");
        }

        private static IEnumerable<ConditionalReferences> GetConditionalReferences(string projectFolder, IEnumerable<ProjectItemGroupElement> itemGroups)
        {
            var conditionItemGroups = itemGroups
                .Select(grp => new
                {
                    grp.Condition,
                    grp.Items
                })
                .GroupBy(grp => grp.Condition);

            foreach (var itemGroup in conditionItemGroups)
            {
                var items = itemGroup.SelectMany(value => value.Items).ToList();

                var projectReferences = GetProjectReferences(projectFolder, items);
                var packageReferences = GetPackageReferences(items);

                var conditionalReferences = new ConditionalReferences
                {
                    Condition = itemGroup.Key,
                    ProjectReferences = projectReferences,
                    PackageReferences = packageReferences
                };

                yield return conditionalReferences;
            }
        }

        private static IReadOnlyCollection<ProjectReference> GetProjectReferences(string projectFolder, IEnumerable<ProjectItemElement> projectItems)
        {
            return projectItems
                .Where(item => item.ItemType.Equals("ProjectReference", StringComparison.OrdinalIgnoreCase))
                .Select(item => 
                {
                    var projectPath = FileUtils.GetAbsolutePath(projectFolder, item.Include);

                    //ProjectRootElement projectRootElement = ProjectRootElement.Open(projectPath);

                    return new ProjectReference
                    {
                        Path = projectPath
                    };
                })
                .ToList();
        }

        private static IReadOnlyCollection<PackageReference> GetPackageReferences(IEnumerable<ProjectItemElement> projectItems)
        {
            return projectItems
                .Where(item => item.ItemType.Equals("PackageReference", StringComparison.OrdinalIgnoreCase))
                .Select(item => new PackageReference
                {
                    Name = item.Include,
                    Version = item.Metadata.SingleOrDefault(item => item.Name == "Version")?.Value
                })
                .ToList();
        }
    }
}