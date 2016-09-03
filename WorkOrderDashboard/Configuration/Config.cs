using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace WorkOrderDashboard.Configuration
{
    public class Config
    {
        public FishbowlConfig Fishbowl { get; set; }
      
        public Config()
        {
            this.Fishbowl = new FishbowlConfig();
      
        }

        public static void Save(Config cfg)
        {
            SaveToDisk(AppDomain.CurrentDomain.BaseDirectory + "config.json", cfg);
        }

        public static void SaveToDisk(String filename, Config config)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Include;
            serializer.Formatting = Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(filename))
            {
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    serializer.Serialize(sw, config);
                }
            }
        }

        public static Config Load()
        {
            var cfg = LoadFromDisk(AppDomain.CurrentDomain.BaseDirectory + "config.json");
            return cfg;
        }
        public static Config LoadFromDisk(String filename)
        {
            if (File.Exists(filename))
            {

                String json = File.ReadAllText(filename);
                var cc = JsonConvert.DeserializeObject<Config>(json);
                return cc;
            }
            else
            {
                return new Config()
                {
                };
            }
        }

    }

    public class FishbowlConfig
    {

        public Int32 FBIAKey { get; set; }
        public string FBIAName { get; set; }
        public string FBIADesc { get; set; }
        public string ServerAddress { get; set; }
        public Int32 ServerPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Persistent { get; set; }

        public String Host { get; set; }
        public int Port { get; set; }
        public String User { get; set; }
        public String Pass { get; set; }
        public String Database { get; set; }
    }
}
