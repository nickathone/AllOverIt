using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Extensions
{
  public static class EnumerableExtensions
  {
    public static IList<TType> AsList<TType>(this IEnumerable<TType> items)
    {
      if (items == null)
      {
        throw new ArgumentNullException(nameof(items));
      }

      return items is IList<TType> list
        ? list
        : items.ToList();
    }

    public static IReadOnlyList<TType> AsReadOnlyList<TType>(this IEnumerable<TType> items)
    {
      if (items == null)
      {
        throw new ArgumentNullException(nameof(items));
      }

      return items is IReadOnlyList<TType> list
        ? list
        : items.ToList();
    }
  }
}