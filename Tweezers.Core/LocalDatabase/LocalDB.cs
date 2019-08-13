using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Tweezers.DBConnector;

namespace Tweezers.LocalDatabase
{
    //TODO
    // ReSharper disable once InconsistentNaming
    public class LocalDB : IDatabaseProxy
    {
        private static LocalDB _instance;
        private Dictionary<string, List<JObject>> _localDb = new Dictionary<string, List<JObject>>();

        public static LocalDB Instance => _instance = _instance ?? new LocalDB();

        private LocalDB()
        {
        }

        public JObject Get(string collection, string id)
        {
            if (!_localDb.ContainsKey(collection))
            {
                _localDb[collection] = new List<JObject>();
            }

            return _localDb[collection].FirstOrDefault(item => item["_id"].ToString() == id);
        }

        public JObject Add(string collection, string id, JObject data)
        {
            if (!_localDb.ContainsKey(collection))
            {
                _localDb[collection] = new List<JObject>();
            }

            JObject newData = data;
            newData["_id"] = id;
            _localDb[collection].Add(data);

            return Get(collection, id);
        }

        public JObject Edit(string collection, string id, JObject data)
        {
            if (!_localDb.ContainsKey(collection))
            {
                _localDb[collection] = new List<JObject>();
            }

            JObject objData = Get(collection, id);
            foreach ((string key, JToken value) in data)
            {
                objData[key] = value;
            }

            Delete(collection, id);
            return Add(collection, id, objData);
        }

        public bool Delete(string collection, string id)
        {
            if (!_localDb.ContainsKey(collection))
            {
                _localDb[collection] = new List<JObject>();
            }

            return _localDb[collection].Remove(Get(collection, id));
        }

        public TweezersMultipleResults<JObject> List(string collection, FindOptions<JObject> opts)
        {
            if (!_localDb.ContainsKey(collection))
            {
                _localDb[collection] = new List<JObject>();
            }

            IEnumerable<JObject> allResults = _localDb[collection]
                .Where(obj => opts.Predicate.Invoke(obj)).ToArray();
            IEnumerable<JObject> results = allResults
                .Skip(opts.Skip)
                .Take(opts.Take);
            int count = allResults.Count();

            return TweezersMultipleResults<JObject>.Create(results, count);
        }

        public IEnumerable<string> GetCollections()
        {
            return _localDb.Keys;
        }
    }
}