using System.Collections.Generic;
using System.Linq;

namespace Schema.Common
{
    public static class ExtensionMethods
    {
        public static bool In<T>(this T obj, IEnumerable<T> collection)
        {
            return collection.Contains(obj);
        }

        public static string ToArrayString<T>(this IEnumerable<T> collection, string delimiter = ", ")
        {
            return string.Join(delimiter, collection);
        }
    }
}
