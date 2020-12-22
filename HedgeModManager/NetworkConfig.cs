﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HedgeModManager
{
    public class NetworkConfig
    {
        public DateTime LastUpdated { get; set; }

        public List<string> URLBlockList { get; set; } = new List<string>();

        public static NetworkConfig LoadConfig(string updateURL, bool force = false)
        {
            try
            {
                NetworkConfig config = null;
                string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "NeverFinishAnything/HedgeModManager/HMMNetworkConfig.json");
                bool update = force || !File.Exists(configPath);
                if (!update && File.Exists(configPath))
                {
                    config = JsonConvert.DeserializeObject<NetworkConfig>(File.ReadAllText(configPath));
                    if (DateTime.Now > config.LastUpdated.AddDays(7))
                        update = true;
                }

                if (update)
                {
                    using (var client = new WebClient())
                    {
                        client.Headers.Add("user-agent", HedgeApp.WebRequestUserAgent);
                        config = JsonConvert.DeserializeObject<NetworkConfig>(client.DownloadString(updateURL));
                        config.LastUpdated = DateTime.Now;
                        string dir = Path.GetDirectoryName(configPath) ?? "HedgeModManager";
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
                    }
                }

                return config;
            }
            catch
            {
                return new NetworkConfig();
            }
        }

    }
}
