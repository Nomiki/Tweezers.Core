using System;
using System.Linq;
using System.Reflection;
using Discoveries.Attributes;
using Discoveries.Enums;

namespace Discoveries.Containers
{
    public sealed class PropertyMetadata
    {
        public PropertyMetadata(PropertyInfo p)
        {
            PropertyName = p.Name;
            DisplayName = p.GetCustomAttributes(typeof(DisplayNameAttribute))
                              .Cast<DisplayNameAttribute>()
                              .SingleOrDefault()?.DisplayName ?? p.Name;

            PropertyType = p.GetType().ToPropertyType();
        }

        public string PropertyName { get; set; }

        public string DisplayName { get; set; }
        
        public PropertyType PropertyType { get; set; }
    }
}
