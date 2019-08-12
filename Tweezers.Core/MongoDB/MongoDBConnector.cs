using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using Tweezers.DBConnector;
using SortDirection = Tweezers.DBConnector.SortDirection;

// ReSharper disable InconsistentNaming

namespace Tweezers.MongoDB
{
    public class MongoDBConnector : IDatabaseProxy
    {
        private const string idField = "_id";

        private readonly Dictionary<string, IMongoCollection<BsonDocument>> collections = new Dictionary<string, IMongoCollection<BsonDocument>>();

        private DBConnectionDetails ConnectionDetails { get; }

        private MongoClientSettings Settings { get; }

        private MongoClient _client;
        private MongoClient Client => _client ?? (_client = new MongoClient(Settings));

        private IMongoDatabase _database;
        private IMongoDatabase Database => _database ?? (_database = Client.GetDatabase(ConnectionDetails.DBName));

        public MongoDBConnector(DBConnectionDetails connectionDetails)
        {
            ConnectionDetails = connectionDetails;
            Settings = new MongoClientSettings()
            {
                ConnectionMode = ConnectionMode.Automatic,
                Server = new MongoServerAddress(connectionDetails.Host, connectionDetails.Port)
            };

            if (!string.IsNullOrWhiteSpace(connectionDetails.Username))
            {
                Settings.Credential = MongoCredential.CreateCredential(connectionDetails.DBName,
                    connectionDetails.Username, connectionDetails.Password);
            }
        }

        public JObject Get(string collection, string id)
        {
            IAsyncCursor<BsonDocument> cursor = GetCollection(collection)
                .FindSync(obj => obj[idField] == BsonValue.Create(id));

            return cursor.FirstOrDefault()?.ToJObject();
        }

        public JObject Add(string collection, string id, JObject data)
        {
            JObject dataToInsert = (JObject) data.DeepClone();
            dataToInsert[idField] = id;

            GetCollection(collection).InsertOne(dataToInsert.ToBsonDocument());

            return Get(collection, id);
        }

        public JObject Edit(string collection, string id, JObject data)
        {
            JObject beforeUpdate = Get(collection, id);
            JObject clone = (JObject) beforeUpdate.DeepClone();
            foreach (var (key, value) in data)
            {
                if (data[key] != null)
                {
                    clone[key] = data[key];
                }
            }

            BsonDocument doc = clone.ToBsonDocument();
            UpdateDefinition<BsonDocument> updateDef = new ObjectUpdateDefinition<BsonDocument>(doc);
            BsonDocument update = GetCollection(collection)
                .FindOneAndUpdate(obj => obj[idField] == BsonValue.Create(id), updateDef);

            return update.ToJObject();
        }

        public bool Delete(string collection, string id)
        {
            BsonDocument delete = GetCollection(collection)
                .FindOneAndDelete(obj => obj[idField] == BsonValue.Create(id));

            DBConnector.FindOptions<JObject> myItem = new DBConnector.FindOptions<JObject>()
            {
                Predicate = obj => obj[idField].ToString() == id
            };

            return !List(collection, myItem).Any();
        }

        public IEnumerable<JObject> List(string collection, DBConnector.FindOptions<JObject> opts)
        {
            IMongoCollection<BsonDocument> mongoCollection = GetCollection(collection);

            IAsyncCursor<BsonDocument> cursor = mongoCollection.FindSync(FilterDefinition<BsonDocument>.Empty);

            IEnumerable<JObject> objects = cursor.ToEnumerable()
                .Where(bson => opts.Predicate.Invoke(bson.ToJObject()))
                .Skip(opts.Skip)
                .Select(bson => bson.ToJObject())
                .Take(opts.Take);

            if (string.IsNullOrWhiteSpace(opts.SortField))
                return objects;

            return opts.SortDirection == SortDirection.Ascending 
                ? objects.OrderBy(obj => obj[opts.SortField]) 
                : objects.OrderByDescending(obj => obj[opts.SortField]);
        }

        public IEnumerable<string> GetCollections()
        {
            return Database.ListCollectionNames().ToEnumerable();
        }

        public IMongoCollection<BsonDocument> GetCollection(string name)
        {
            if (!collections.ContainsKey(name))
            {
                collections[name] = Database.GetCollection<BsonDocument>(name);
            }

            return collections[name];
        }
    }
}
