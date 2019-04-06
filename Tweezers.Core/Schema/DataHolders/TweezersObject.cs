using System.Collections.Generic;

namespace Schema.DataHolders
{
    public sealed class TweezersObject
    {
        public string CollectionName { get; set; }

        public TweezersDisplayNames DisplayNames { get; } = new TweezersDisplayNames();

        public Dictionary<string, TweezersField> Fields { get; } = new Dictionary<string, TweezersField>();
    }
}
