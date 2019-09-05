using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tweezers.Schema.DataHolders
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DefaultPermission
    {
        None,
        View,
        Edit
    }
}