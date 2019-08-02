using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Tweezers.Api.Identity;
using Tweezers.Api.Schema;
using Tweezers.Api.Utils;
using Tweezers.DBConnector;
using Tweezers.LocalDatabase;
using Tweezers.Schema.DataHolders;
using Tweezers.MongoDB;

namespace Tweezers.MetadataManagement
{
    public sealed class TweezersMetadata
    {
        private static TweezersMetadata _instance;

        public TweezersDetails TweezersDetails { get; set; }

        [JsonProperty("dbDetails")]
        // ReSharper disable once InconsistentNaming
        public DBConnectionDetails DBDetails { get; set; }

        public List<TweezersObject> Schema { get; set; }

        public static TweezersMetadata Instance => _instance;

        public static TweezersMetadata Init(string fileName)
        {
            string settings = File.ReadAllText(fileName);
            TweezersMetadata metadata = settings.Deserialize<TweezersMetadata>();
            TweezersSchemaFactory.DatabaseProxy = GetDatabaseProxyInstance(metadata.DBDetails);

            SchemaManagement.CanChangeSchema = metadata.TweezersDetails.CanChangeSchema;

            if (metadata.TweezersDetails.UseIdentity)
            {
                IdentityManager.RegisterIdentity();
            }

            foreach (TweezersObject obj in metadata.Schema)
            {
                TweezersSchemaFactory.AddObject(obj);
            }

            _instance = metadata;

            return _instance;
        }

        private static IDatabaseProxy GetDatabaseProxyInstance(DBConnectionDetails dbDetails)
        {
            switch (dbDetails.DBType)
            {
                case "MongoDB": return new MongoDBConnector(dbDetails);
                case "LocalDB": return LocalDB.Instance;
                default: return FindCustomDatabaseProxy(dbDetails); 
            }
        }

        private static IDatabaseProxy FindCustomDatabaseProxy(DBConnectionDetails dbDetails)
        {
            Type iDatabaseProxyType = typeof(IDatabaseProxy);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type dbProxyType = assemblies
                .SelectMany(s => s.GetTypes())
                .SingleOrDefault(p => iDatabaseProxyType.IsAssignableFrom(p) && p.Name.Equals(dbDetails.DBType));

            if (dbProxyType == null)
            {
                throw new ArgumentException($"Could not find db of type {dbDetails.DBType}");
            }

            return (IDatabaseProxy) Activator.CreateInstance(dbProxyType, dbDetails);
        }
    }
}
