using AllOverIt.Extensions;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AllOverItDependencyDiagram.Parser
{
    internal sealed class NugetPackageReferencesResolver
    {
        private readonly IDictionary<(string, string), IEnumerable<PackageReference>> _nugetCache = new Dictionary<(string, string), IEnumerable<PackageReference>>();
        private readonly int _maxDepth;

        public NugetPackageReferencesResolver(int maxDepth = 1)
        {
            _maxDepth = maxDepth;
        }

        public Task<IReadOnlyCollection<PackageReference>> GetPackageReferences(string packageName, string packageVersion)
        {
            return GetPackageReferencesRecursively(packageName, packageVersion, 1);
        }

        private async Task<IReadOnlyCollection<PackageReference>> GetPackageReferencesRecursively(string packageName, string packageVersion, int depth)
        {
            if (depth > _maxDepth)
            {
                return Array.Empty<PackageReference>();
            }

            packageVersion = GetAssumedVersion(packageVersion);

            var cacheKey = (packageName, packageVersion);

            if (!_nugetCache.TryGetValue(cacheKey, out var packageReferences))
            {
                var apiUrl = $"https://api.nuget.org/v3-flatcontainer/{packageName}/{packageVersion}/{packageName}.nuspec";
                var nuspecXml = await apiUrl.GetStringAsync();
                var nuspec = XDocument.Parse(nuspecXml);

                var ns = nuspec.Root.Name.Namespace;

                var dependenciesByFramework = nuspec.Descendants(ns + "group")
                    .Where(grp => grp.Attribute("targetFramework") != null) // ? the targetFramework attribute is always present
                    .GroupBy(grp => grp.Attribute("targetFramework").Value)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Descendants(ns + "dependency")
                                  .SelectAsReadOnlyCollection(element => new
                                  {
                                      Id = element.Attribute("id").Value,
                                      Version = element.Attribute("version").Value
                                  })
                    );

                if (dependenciesByFramework.Any())
                {
                    var packageReferencesList = new List<PackageReference>();

                    // =====
                    // take the last for now - until everything is actually grouped (or constrained) by the target framework
                    // =====

                    foreach (var dependency in dependenciesByFramework.Last().Value)
                    {
                        var dependencyName = dependency.Id;
                        var dependencyVersion = GetAssumedVersion(dependency.Version);

                        var transitiveReferences = await GetPackageReferencesRecursively(dependencyName, dependencyVersion, depth + 1);

                        var packageReference = new PackageReference(true, depth)
                        {
                            Name = dependencyName,
                            Version = dependencyVersion,
                            TransitiveReferences = transitiveReferences.AsReadOnlyCollection()
                        };

                        packageReferencesList.Add(packageReference);
                    }

                    packageReferences = packageReferencesList;
                }

                _nugetCache.Add(cacheKey, packageReferences);
            }

            return packageReferences?.AsReadOnlyCollection() ?? AllOverIt.Collections.Collection.EmptyReadOnly<PackageReference>();
        }

        private static string GetAssumedVersion(string packageVersion)
        {
            if (packageVersion[0] == '[')
            {
                // Need to handle versions such as [2.1.1, 3.0.0)
                packageVersion = packageVersion[1..^1].Split(",").First().Trim();
            }

            return packageVersion;
        }
    }
}