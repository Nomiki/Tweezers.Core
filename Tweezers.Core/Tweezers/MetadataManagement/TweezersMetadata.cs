using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Tweezers.Identity;
using Tweezers.LocalDatabase;
using Tweezers.Schema.DataHolders;

namespace Tweezers.MetadataManagement
{
    public sealed class TweezersMetadata
    {
        private static TweezersMetadata _instance;

        public bool UseIdentity { get; set; }

        public string Title { get; set; }

        public List<TweezersObject> Schema { get; set; }

        public static TweezersMetadata Instance => _instance;

        public static TweezersMetadata Init(string fileName)
        {
            TweezersSchemaFactory.DatabaseProxy = LocalDB.Instance;

            string settings = File.ReadAllText(fileName);
            TweezersMetadata metadata = JsonConvert.DeserializeObject<TweezersMetadata>(settings);

            if (metadata.UseIdentity)
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
    }
}
