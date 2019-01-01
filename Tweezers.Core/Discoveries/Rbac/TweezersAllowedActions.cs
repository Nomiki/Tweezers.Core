using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tweezers.Discoveries.Rbac
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TweezersAllowedActions
    {
        None,
        View,
        Edit,
        Admin
    }
}
