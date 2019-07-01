using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using Tweezers.DBConnector;
// ReSharper disable InconsistentNaming

namespace Tweezers.MongoDB
{
    public class MongoDBConnector : IDatabaseProxy
    {
        private IMongoDatabase database;
        //TODO: Move
        private const string idField = "_id";

        private Dictionary<string, IMongoCollection<JObject>> collections = new Dictionary<string, IMongoCollection<JObject>>();

        public MongoDBConnector(DBConnectionDetails connectionDetails)
        {
            MongoClient client = new MongoClient(new MongoClientSettings()
            {
                Credential = MongoCredential.CreateCredential(connectionDetails.DBName, connectionDetails.Username,
                    connectionDetails.Password),
                ConnectionMode = ConnectionMode.Automatic,
                Server = new MongoServerAddress(connectionDetails.Host, connectionDetails.Port)
            });

            database = client.GetDatabase(connectionDetails.DBName);
        }

        public JObject Get(string collection, string id)
        {
            return GetCollection(collection).Find(obj => obj[idField].ToString() == id).FirstOrDefault();
        }

        public JObject Add(string collection, string id, JObject data)
        {
            JObject dataToInsert = (JObject) data.DeepClone();
            dataToInsert[idField] = id;
            GetCollection(collection).InsertOne(data);

            return Get(collection, id);
        }

        public JObject Edit(string collection, string id, JObject data)
        {
            UpdateDefinition<JObject> updateDef = new ObjectUpdateDefinition<JObject>(data);
            
            JObject update = GetCollection(collection).FindOneAndUpdate(obj => obj[idField].ToString() == id, updateDef);

            return update;
        }

        public bool Delete(string collection, string id)
        {
            JObject delete = GetCollection(collection).FindOneAndDelete(obj => obj[idField].ToString() == id);

            DBConnector.FindOptions<JObject> myItem = new DBConnector.FindOptions<JObject>()
            {
                Predicate = obj => obj[idField].ToString() == id
            };

            return !List(collection, myItem).Any();
        }

        public IEnumerable<JObject> List(string collection, DBConnector.FindOptions<JObject> opts)
        {
            //TODO Ask
            return GetCollection(collection)
                .Find(obj => opts.Predicate.Invoke(obj))
                .Skip(opts.Skip)
                .Limit(opts.Take)
                .ToEnumerable();
        }

        public IEnumerable<string> GetCollections()
        {
            return database.ListCollectionNames().ToEnumerable();
        }

        public IMongoCollection<JObject> GetCollection(string name)
        {
            if (!collections.ContainsKey(name))
            {
                collections[name] = database.GetCollection<JObject>(name);
            }

            return collections[name];
        }
    }
}
