using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Tweezers.Schema.DataHolders.DB;

namespace Tweezers.Schema.Interfaces
{
    public interface IDatabaseProxy
    {
        JObject Get(string collection, string id);

        JObject Add(string collection, string id, JObject data);

        JObject Edit(string collection, string id, JObject data);

        bool Delete(string collection, string id);

        IEnumerable<JObject> List(string collection, FindOptions<JObject> opts);

        IEnumerable<string> GetCollections();
    }
}
