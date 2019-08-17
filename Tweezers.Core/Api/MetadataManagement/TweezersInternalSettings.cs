using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Tweezers.Api.Identity;
using Tweezers.Api.Identity.Controllers;
using Tweezers.Api.Identity.DataHolders;
using Tweezers.Api.Schema;
using Tweezers.Api.Utils;
using Tweezers.DBConnector;
using Tweezers.LocalDatabase;
using Tweezers.MongoDB;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Api.MetadataManagement
{
    public sealed class TweezersInternalSettings
    {
        private static readonly string internalSettingsFileName = "tweezers-internal-settings.json";

        public TweezersDetails AppDetails { get; set; }

        [JsonProperty("dbDetails")]
        // ReSharper disable once InconsistentNaming
        public DBConnectionDetails DBDetails { get; set; }

        public List<TweezersObject> Schema { get; set; }

        public static TweezersInternalSettings Instance { get; private set; }

        public static TweezersInternalSettings Init()
        {
            if (File.Exists(internalSettingsFileName))
            {
                string settings = File.ReadAllText(internalSettingsFileName);
                TweezersInternalSettings internalSettings = settings.Deserialize<TweezersInternalSettings>();
                TweezersSchemaFactory.DatabaseProxy = GetDatabaseProxyInstance(internalSettings.DBDetails);

                SchemaManagement.CanChangeSchema = internalSettings.AppDetails.CanChangeSchema;

                if (internalSettings.AppDetails.UseIdentity)
                {
                    IdentityManager.RegisterIdentity();
                    if (!TweezersRuntimeSettings.Instance.IsInitialized)
                    {
                        UsersController.CreateUser(new LoginRequest()
                        {
                            Username = internalSettings.AppDetails.InitialUsername,
                            Password = internalSettings.AppDetails.InitialPassword
                        });
                    }
                }

                if (internalSettings.Schema != null)
                {
                    foreach (TweezersObject obj in internalSettings.Schema)
                    {
                        if (TweezersSchemaFactory.Find(obj.CollectionName, safe: true) == null)
                            TweezersSchemaFactory.AddObject(obj);
                    }
                }

                Instance = internalSettings;
            }

            return Instance;
        }

        public static void WriteToSettingsFile(TweezersInternalSettings settings)
        {
            using (FileStream fs = new FileStream(internalSettingsFileName, FileMode.OpenOrCreate))
            {
                fs.SetLength(0);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(settings.Serialize());
                }
            }
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