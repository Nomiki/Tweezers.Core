using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Common;
using Tweezers.Discoveries.Enums;

namespace Tweezers.Discoveries.Containers
{
    public sealed class PropertyMetadata
    {
        public PropertyMetadata(PropertyInfo p)
        {
            PropertyName = char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1);
            TweezersFieldAttribute fieldAttribute = p.GetCustomAttributes(typeof(TweezersFieldAttribute))
                .Cast<TweezersFieldAttribute>()
                .SingleOrDefault();
            DisplayName = fieldAttribute?.DisplayName ?? p.Name;
            IdField = p.GetCustomAttributes<TweezersIdAttribute>().Any();
            PropertyType = p.PropertyType.ToPropertyType();
            if (PropertyType == PropertyType.Enum)
            {
                Values = p.PropertyType.EnumValues();
            }

            else if (PropertyType == PropertyType.String && 
                     fieldAttribute?.Values?.Length > 0)
            {
                Values = fieldAttribute.Values.ToDictionary(v => v, v => v as object);
            }
        }

        public bool IdField { get; set; }

        public string PropertyName { get; set; }

        public string DisplayName { get; set; }
        
        public PropertyType PropertyType { get; set; }

        public Dictionary<string, object> Values { get; set; }
    }
}
