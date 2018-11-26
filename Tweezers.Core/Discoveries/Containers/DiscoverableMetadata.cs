using System.Collections.Generic;

namespace Discoveries.Containers
{
    public sealed class DiscoverableMetadata
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public List<PropertyMetadata> PropertyData { get; set; }
    }
}
