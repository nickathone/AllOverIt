﻿using System;
using System.Linq;
using System.Reflection;

namespace GCCollectorStartupHook
{
    // NOTE: This code has been copied (and modified to remove pre-condition checks) from AllOverIt.Plugin as dependencies are not supported in startup hooks.
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

            var requiredType = assembly.ExportedTypes.FirstOrDefault(type => type.FullName == typeName);

            return Activator.CreateInstance(requiredType) as TType;
        }
    }
}