using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace Tweezers.MongoDB
{
    internal static class TweezersMongoExtensions
    {
        public static JObject ToJObject(this BsonDocument bson)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(bson.ToJson());
        }

        public static BsonDocument ToBsonDocument(this JObject jObject)
        {
            return BsonDocument.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(jObject));
        }
    }
}
