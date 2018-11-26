using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Discoveries.Enums
{
    public static class Enum<T> where T : struct, IConvertible
    {
        public static IEnumerable<T> Values
        {
            get
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException($"{nameof(T)} is not an enumerated type");

                foreach (object value in typeof(T).GetEnumValues())
                {
                    if (value is T)
                    {
                        yield return (T)value;
                    }
                }
            }
        }

        public static T ValueOf(string value)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"{nameof(T)} is not an enumerated type");

            return Values.FirstOrDefault(v => v.ToString(CultureInfo.CurrentCulture).Equals(value));
        }

        public static T DecimalValueOf(string value)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"{nameof(T)} is not an enumerated type");

            int intValue = Convert.ToInt32(value);

            return Values.FirstOrDefault(v => Convert.ToInt32(v) == intValue);
        }
    }
}