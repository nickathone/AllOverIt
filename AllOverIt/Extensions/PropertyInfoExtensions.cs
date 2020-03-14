using System.Reflection;

namespace AllOverIt.Extensions
{
  public static class PropertyInfoExtensions
  {
    public static bool IsAbstract(this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetMethod.IsAbstract;
    }

    public static bool IsInternal(this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetMethod.IsAssembly;
    }

    public static bool IsPrivate(this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetMethod.IsPrivate;
    }

    public static bool IsProtected(this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetMethod.IsFamily;
    }

    public static bool IsPublic(this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetMethod.IsPublic;
    }

    public static bool IsStatic(this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetMethod.IsStatic;
    }

    public static bool IsVirtual(this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetMethod.IsVirtual;
    }
  }
}