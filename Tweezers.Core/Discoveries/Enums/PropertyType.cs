using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tweezers.Discoveries.Common;

namespace Tweezers.Discoveries.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PropertyType
    {
        [EnumMember(Value = "Number")]
        Number,

        [EnumMember(Value = "String")]
        String,

        [EnumMember(Value = "Boolean")]
        Boolean,

        [EnumMember(Value = "Array")]
        Array,

        [EnumMember(Value = "Object")]
        Object,
    }

    internal static class PropertyTypeHelper
    {
        private static readonly TypeCode[] NumberTypes =
        {
            TypeCode.Byte, TypeCode.SByte, TypeCode.UInt16, TypeCode.UInt32, TypeCode.UInt64, TypeCode.Int16,
            TypeCode.Int32, TypeCode.Int64, TypeCode.Decimal, TypeCode.Double, TypeCode.Single
        };

        private static readonly TypeCode[] StringTypes = {TypeCode.Char, TypeCode.String};

        private static readonly TypeCode[] BooleanTypes = {TypeCode.Boolean};

        public static PropertyType ToPropertyType(this Type t)
        {
            TypeCode typeCode = Type.GetTypeCode(t);
            if (typeCode.In(NumberTypes))
                return PropertyType.Number;

            if (typeCode.In(StringTypes))
                return PropertyType.String;

            if (typeCode.In(BooleanTypes))
                return PropertyType.Boolean;

            if (IsArrayType(t)) 
                return PropertyType.Array;

            return PropertyType.Object;
        }

        private static bool IsArrayType(Type t)
        {
            return t.GetInterface("IEnumerable") != null && t != typeof(string);
        }
    }
}