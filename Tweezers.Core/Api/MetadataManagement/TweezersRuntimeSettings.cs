using System;
using System.IO;
using MongoDB.Bson.IO;
using Tweezers.Api.Utils;

namespace Tweezers.Api.MetadataManagement
{
    public class TweezersRuntimeSettings
    {
        private static readonly string runtimeSettingsFileName = "tweezers-runtime-settings.json";

        public int Port { get; set; } = 5001;

        public bool UseSsl { get; set; } = true;

        public bool IsInitialized { get; set; }

        private static TweezersRuntimeSettings _instance;

        public static TweezersRuntimeSettings Instance => _instance ?? (_instance = Init());

        private static TweezersRuntimeSettings Init()
        {
            if (File.Exists(runtimeSettingsFileName))
            {
                string settingFileText = File.ReadAllText(runtimeSettingsFileName);
                return settingFileText.Deserialize<TweezersRuntimeSettings>();
            }

            TweezersRuntimeSettings defaultSettings = new TweezersRuntimeSettings();
            defaultSettings.WriteToSettingsFile();
            return defaultSettings;
        }

        public void WriteToSettingsFile()
        {
            using (FileStream fs = new FileStream(runtimeSettingsFileName, FileMode.OpenOrCreate))
            {
                fs.SetLength(0);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(this.Serialize());
                }
            }
        }
    }
}
