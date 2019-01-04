using System;
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

        public static Dictionary<int, string> EnumValues(this Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException($"{t.Name} is not an enumerated type");

            Dictionary<int, string> result = new Dictionary<int, string>();
            foreach (object value in t.GetEnumValues())
            {
                if (value.GetType() == t)
                {
                    result[(int) value] = value.ToString();
                }
            }

            return result;
        }
    }
}