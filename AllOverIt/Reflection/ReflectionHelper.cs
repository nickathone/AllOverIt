using AllOverIt.Extensions;
using System.Collections.Generic;
using System.Reflection;

namespace AllOverIt.Reflection
{
  public static class ReflectionHelper
  {
    /// <summary>
    /// Gets the <see cref="PropertyInfo"/> (property metadata) for a given property on a <typeparam name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type to obtain the property metadata from.</typeparam>
    /// <param name="propertyName">The name of the property to obtain metadata for.</param>
    /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified property on the specified <typeparam name="TType"/>.</returns>
    /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
    /// by <typeparam name="TType"/>.</remarks>
    public static PropertyInfo GetPropertyInfo<TType>(string propertyName)
    {
      return typeof(TType).GetPropertyInfo(propertyName);
    }

    /// <summary>
    /// Gets <see cref="PropertyInfo"/> (property metadata) for a given <typeparam name="TType"/> and binding option.
    /// </summary>
    /// <typeparam name="TType">The type to obtain property metadata for.</typeparam>
    /// <param name="binding">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
    /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
    /// If false, only property metadata of the declared type is returned.</param>
    /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a provided <typeparam name="TType"/>.</returns>
    /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
    /// by <typeparam name="TType"/>.</remarks>
    public static IEnumerable<PropertyInfo> GetPropertyInfo<TType>(BindingOptions binding = BindingOptions.Default, bool declaredOnly = false)
    {
      return typeof(TType).GetPropertyInfo(binding, declaredOnly);
    }

    /// <summary>
    /// Gets <see cref="MethodInfo"/> (method metadata) for a given <typeparam name="TType"/> and binding option.
    /// </summary>
    /// <typeparam name="TType">The type to obtain method metadata for.</typeparam>
    /// <param name="binding">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
    /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
    /// If false, only method metadata of the declared type is returned.</param>
    /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a provided <typeparam name="TType"/>.</returns>
    /// <remarks>When class inheritance is involved, this method returns the first method found, starting at the type represented
    /// by <typeparam name="TType"/>.</remarks>
    public static IEnumerable<MethodInfo> GetMethodInfo<TType>(BindingOptions binding = BindingOptions.Default, bool declaredOnly = false)
    {
      return typeof(TType).GetMethodInfo(binding, declaredOnly);
    }
  }
}