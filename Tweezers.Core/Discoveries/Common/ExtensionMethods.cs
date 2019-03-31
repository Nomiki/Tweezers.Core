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

        public static Dictionary<string, object> EnumValues(this Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException($"{t.Name} is not an enumerated type");

            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (object value in t.GetEnumValues())
            {
                if (value.GetType() == t)
                {
                    result[value.ToString()] = (int)value;
                }
            }

            return result;
        }

        public static TimeSpan Hours(this int i)
        {
            return TimeSpan.FromHours(i);
        }
    }
}