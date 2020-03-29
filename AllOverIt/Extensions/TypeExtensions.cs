using AllOverIt.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AllOverIt.Extensions
{
  public static class TypeExtensions
  {
    /// <summary>
    /// Gets the <see cref="PropertyInfo"/> (property metadata) for a given property on a <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to obtain the property metadata from.</param>
    /// <param name="propertyName">The name of the property to obtain metadata for.</param>
    /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a specified property on the provided <param name="type"></param>.</returns>
    /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
    /// by <param name="type"></param>.</remarks>
    public static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
    {
      return TypeInfoExtensions.GetPropertyInfo(type.GetTypeInfo(), propertyName);
    }

    /// <summary>
    /// Gets <see cref="PropertyInfo"/> (property metadata) for a given <see cref="Type"/> and binding option.
    /// </summary>
    /// <param name="type">The type to obtain property metadata for.</param>
    /// <param name="binding">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
    /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
    /// If false, only property metadata of the declared type is returned.</param>
    /// <returns>The property metadata, as <see cref="PropertyInfo"/>, of a provided <see cref="Type"/>.</returns>
    /// <remarks>When class inheritance is involved, this method returns the first property found, starting at the type represented
    /// by <param name="type"></param>.</remarks>
    public static IEnumerable<PropertyInfo> GetPropertyInfo(this Type type, BindingOptions binding = BindingOptions.Default, bool declaredOnly = false)
    {
      var predicate = BindingOptionsHelper.BuildBindingPredicate(binding);
      var typeInfo = type.GetTypeInfo();

      return from propInfo in typeInfo.GetPropertyInfo(declaredOnly)
             let methodInfo = propInfo.GetMethod
             where predicate.Invoke(methodInfo)
             select propInfo;
    }

    /// <summary>
    /// Gets <see cref="MethodInfo"/> (method metadata) for a given <see cref="Type"/> and binding option.
    /// </summary>
    /// <param name="type">The type to obtain method metadata for.</param>
    /// <param name="binding">The binding option that determines the scope, access, and visibility rules to apply when searching for the metadata.</param>
    /// <param name="declaredOnly">If true, the metadata of properties in the declared class as well as base class(es) are returned.
    /// If false, only method metadata of the declared type is returned.</param>
    /// <returns>The method metadata, as <see cref="MethodInfo"/>, of a provided <see cref="Type"/>.</returns>
    /// <remarks>When class inheritance is involved, this method returns the first method found, starting at the type represented
    /// by <param name="type"></param>.</remarks>
    public static IEnumerable<MethodInfo> GetMethodInfo(this Type type, BindingOptions binding = BindingOptions.Default, bool declaredOnly = false)
    {
      var predicate = BindingOptionsHelper.BuildBindingPredicate(binding);
      var currentType = type;

      while (currentType != null)
      {
        var typeInfo = currentType.GetTypeInfo();

        foreach (var method in typeInfo.DeclaredMethods)
        {
          if (predicate.Invoke(method))
          {
            yield return method;
          }
        }

        currentType = !declaredOnly
          ? typeInfo.BaseType
          : null;
      }
    }

    public static bool IsEnumType(this Type type)
    {
      return type.GetTypeInfo().IsEnum;
    }

    public static bool IsClassType(this Type type)
    {
      return type.GetTypeInfo().IsClass;
    }

    public static bool IsPrimitiveType(this Type type)
    {
      return type.GetTypeInfo().IsPrimitive;
    }

    public static bool IsIntegralType(this Type type)
    {
      return new[] {typeof (byte), typeof (sbyte), typeof (short), typeof (ushort),
                    typeof (int), typeof (uint), typeof (long), typeof (ulong)}.Contains(type);
    }

    public static bool IsEnumerableType(this Type type, bool includeString = false)
    {
      return type == typeof(string)
        ? includeString
        : typeof(IEnumerable).IsAssignableFromType(type);
    }

    public static bool IsGenericEnumerableType(this Type type)
    {
      return type.IsGenericType() && typeof(IEnumerable).IsAssignableFromType(type);
    }

    public static bool IsGenericType(this Type type)
    {
      return type.GetTypeInfo().IsGenericType;
    }

    public static IEnumerable<Type> GetGenericArguments(this Type type)
    {
      return type.GetTypeInfo().GenericTypeArguments;
    }

    public static bool IsAssignableFromType(this Type type, Type fromType)
    {
      var fromTypeInfo = fromType.GetTypeInfo();

      return type.GetTypeInfo().IsAssignableFrom(fromTypeInfo);
    }

    public static bool IsGenericNullableType(this Type type)
    {
      return type.IsGenericType() && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    public static string GetFriendlyName(this Type type)
    {
      if (type.IsGenericType() && !type.IsGenericNullableType())
      {
        var typeName = type.Name;

        typeName = typeName.Remove(typeName.IndexOf('`'));

        var genericTypeNames = from genericArgument in type.GetGenericArguments()
                               select GetFriendlyName(genericArgument);

        var stringBuilder = new StringBuilder();

        stringBuilder.Append(typeName);
        stringBuilder.Append('<');
        stringBuilder.Append(string.Join(", ", genericTypeNames));
        stringBuilder.Append('>');

        return stringBuilder.ToString();
      }

      return type.IsGenericNullableType()
        ? $"{type.GetGenericArguments().Single().Name}?"
        : type.Name;
    }
  }
}