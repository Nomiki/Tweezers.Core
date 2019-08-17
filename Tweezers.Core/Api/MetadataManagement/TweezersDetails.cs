using Newtonsoft.Json;

namespace Tweezers.Api.MetadataManagement
{
    public class TweezersDetails
    {
        [JsonIgnore]
        public bool UseIdentity { get; set; } = true;

        public bool CanChangeSchema { get; set; }

        public string Title { get; set; }

        public string InitialUsername { get; set; }
        
        public string InitialPassword { get; set; }
    }
}
