using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tweezers.Schema.DataHolders
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DefaultPermission
    {
        None = 0,
        View = 1,
        Edit = 2,
    }
}