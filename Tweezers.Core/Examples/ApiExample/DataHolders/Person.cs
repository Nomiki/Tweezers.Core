using Discoveries.Attributes;
using DiscoveryApi.Attributes;

namespace ApiExample.DataHolders
{
    [Discoverable("Person")]
    public class Person
    {
        [TweezersId]
        public string Id { get; set; }

        [DisplayName("Full name")]
        public string Name { get; set; }
        
        [DisplayName("Cat name")]
        public string CatName { get; set; }
    }
}
