using System.Collections.Generic;
using System.Linq;
using Tweezers.DBConnector;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Schema.Discovery
{
    public class SchemaDiscoveryBootstrapper
    {
        private readonly string[] _tweezersInternalSchemas =
            {"tweezers-users", TweezersSchemaFactory.ObjectMetadataCollectionName};

        private IDatabaseProxy DatabaseProxy { get; }

        public SchemaDiscoveryBootstrapper(IDatabaseProxy databaseProxy)
        {
            DatabaseProxy = databaseProxy;
        }

        public void Run()
        {
            IEnumerable<string> collections = DatabaseProxy.GetCollections().Intersect(_tweezersInternalSchemas);

            foreach (string collection in collections)
            {
                TweezersObject obj = TweezersSchemaFactory.Find(collection, false, true, true);
                if (obj == null)
                {
                    // Create TweezersObj instance for collection
                }
            }
        }
    }
}
