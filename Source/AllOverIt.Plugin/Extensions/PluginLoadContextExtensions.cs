using AllOverIt.Assertion;
using AllOverIt.Plugin.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Plugin.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class PluginLoadContextExtensions
    {
        public static void CreateType(this PluginLoadContext loadContext, string assemblyName, string typeName)
        {
            _ = loadContext.CreateType<object>(assemblyName, typeName);
        }

        public static TType CreateType<TType>(this PluginLoadContext loadContext, string assemblyName, string typeName) where TType : class
        {
            var assemblyNameInfo = new AssemblyName(assemblyName);
            var assembly = loadContext.LoadFromAssemblyName(assemblyNameInfo);

            // Assembly.CreateInstance() is not supported with Trimming so
            var requiredType = assembly.ExportedTypes.FirstOrDefault(type => type.FullName == typeName);

            Throw<CannotLoadPluginTypeException>.WhenNull(requiredType, $"The {typeName} type was not found in the {assemblyName} assembly.");

            return Activator.CreateInstance(requiredType) as TType;
        }
    }
}
