using System.Collections.Generic;
using System.Linq;

namespace Tweezers.Discoveries.Common
{
    public static class ExtensionMethods
    {
        public static bool None<T>(this IEnumerable<T> collection)
        {
            return !collection.Any();
        }

        public static bool In<T>(this T obj, IEnumerable<T> collection)
        {
            return collection.Contains(obj);
        }

        public static bool In<T>(this T obj, params T[] collection)
        {
            return collection.Contains(obj);
        }
    }
}
