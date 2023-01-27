#if NETCOREAPP3_1_OR_GREATER


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
        public static object CreateType(this PluginLoadContext loadContext, string assemblyName, string typeName)
        {
            return loadContext.CreateType<object>(assemblyName, typeName);
        }

        public static object CreateType(this PluginLoadContext loadContext, string assemblyName, string typeName, params object[] args)
        {
            return loadContext.CreateType<object>(assemblyName, typeName, args);
        }

        public static TType CreateType<TType>(this PluginLoadContext loadContext, string assemblyName, string typeName) where TType : class
        {
            var requiredType = LoadTypeFromAssembly(loadContext, assemblyName, typeName);

            return Activator.CreateInstance(requiredType) as TType;
        }

        public static TType CreateType<TType>(this PluginLoadContext loadContext, string assemblyName, string typeName, params object[] args) where TType : class
        {
            var requiredType = LoadTypeFromAssembly(loadContext, assemblyName, typeName);

            return Activator.CreateInstance(requiredType, args) as TType;
        }

        // assemblyName must be the DLL name without the extension
        // typeFullName must be namespace.typename
        private static Type LoadTypeFromAssembly(PluginLoadContext loadContext, string assemblyName, string fullTypeName)
        {
            var assemblyNameInfo = new AssemblyName(assemblyName);
            var assembly = loadContext.LoadFromAssemblyName(assemblyNameInfo);

            var requiredType = assembly.ExportedTypes.SingleOrDefault(type => type.FullName == fullTypeName);

            Throw<CannotLoadPluginTypeException>.WhenNull(requiredType, $"The {fullTypeName} type was not found in the {assemblyName} assembly.");

            return requiredType;
        }
    }
}

#endif
