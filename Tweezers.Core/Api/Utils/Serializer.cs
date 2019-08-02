using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Tweezers.Api.Utils
{
    public static class Serializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static readonly JsonSerializer JsonSerializer = JsonSerializer.Create(Settings);

        public static T Deserialize<T>(this string s)
        {
            return JsonConvert.DeserializeObject<T>(s, Settings);
        }

        public static string Serialize<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }
    }
}
