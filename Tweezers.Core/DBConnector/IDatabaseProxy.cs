using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Tweezers.DBConnector
{
    public interface IDatabaseProxy
    {
        JObject Get(string collection, string id);

        JObject Add(string collection, string id, JObject data);

        JObject Edit(string collection, string id, JObject data);

        bool Delete(string collection, string id);

        TweezersMultipleResults<JObject> List(string collection, FindOptions<JObject> opts);

        IEnumerable<string> GetCollections();
    }
}
