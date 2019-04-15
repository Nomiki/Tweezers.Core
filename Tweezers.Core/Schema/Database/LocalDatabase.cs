using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Tweezers.Schema.DataHolders.DB;
using Tweezers.Schema.Exceptions;
using Tweezers.Schema.Interfaces;

namespace Tweezers.Schema.Database
{
    //TODO
    public class LocalDatabase : IDatabaseProxy
    {
        private static LocalDatabase _instance;
        private Dictionary<string, List<JObject>> _localDb = new Dictionary<string, List<JObject>>();

        public static LocalDatabase Instance => _instance = _instance ?? new LocalDatabase();

        private LocalDatabase()
        {
        }

        public JObject Get(string collection, string id)
        {
            if (!_localDb.ContainsKey(collection))
            {
                _localDb[collection] = new List<JObject>();
            }

            return _localDb[collection].First(item => item["_id"].ToString() == id);
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

        public IEnumerable<JObject> List(string collection, FindOptions<JObject> opts)
        {
            if (!_localDb.ContainsKey(collection))
            {
                _localDb[collection] = new List<JObject>();
            }

            return _localDb[collection]
                .Where(obj => opts.Predicate.Invoke(obj))
                .Skip(opts.Skip)
                .Take(opts.Take);
        }
    }
}