using System.Collections.Generic;
using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Rbac;

namespace Tweezers.Discoveries.Containers
{
    public sealed class DiscoverableMetadata
    {
        public string Name { get; set; }

        public TweezersEntityAttribute EntityData { get; set; }

        public List<PropertyMetadata> PropertyData { get; set; }
    }
}
