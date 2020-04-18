using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AllOverIt.Extensions
{
  public static class EnumerableExtensions
  {
    public static IList<TType> AsList<TType>(this IEnumerable<TType> items)
    {
      _ = items ?? throw new ArgumentNullException(nameof(items));

      return items is IList<TType> list
        ? list
        : items.ToList();
    }

    public static IReadOnlyList<TType> AsReadOnlyList<TType>(this IEnumerable<TType> items)
    {
      _ = items ?? throw new ArgumentNullException(nameof(items));

      return items is IReadOnlyList<TType> list
        ? list
        : items.ToList();
    }

    public static IReadOnlyCollection<TType> AsReadOnlyCollection<TType>(this IEnumerable<TType> items)
    {
      _ = items ?? throw new ArgumentNullException(nameof(items));

      return items is IReadOnlyCollection<TType> list
        ? list
        : new ReadOnlyCollection<TType>(items.AsList());
    }

    /// <summary>
    /// Applicable to strings and collections, this method determines if the instance is null or empty.
    /// </summary>
    /// <param name="items">The object of interest.</param>
    /// <returns>True if the object is null or empty.</returns>
    public static bool IsNullOrEmpty(this IEnumerable items)
    {
      return items == null || !items.GetEnumerator().MoveNext();
    }
  }
}