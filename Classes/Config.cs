using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace DiscordWebhookInfo.Classes
{
    public class Config
    {
        private Dictionary<string, string> _config = new Dictionary<string, string>();
        private string _configPath;
        private string _configFile;

        private DateTime LastUpdate;

        public Config()
        {
            _configPath = $"Mods{Path.DirectorySeparatorChar}DiscordWebhookInfo";
            _configFile = $"{Path.DirectorySeparatorChar}Config.xml";
            Load();
        }

        private void Load()
        { //Load Config from file
            _config.Clear();
            if (!File.Exists(_configPath + _configFile))
            {
                try
                {
                    if(!Directory.Exists(_configPath))
                        Directory.CreateDirectory(_configPath);
                    
                    File.WriteAllText(_configPath + _configFile, DiscordWebhookInfo.Properties.Resources.DefaultConfig);
                    ModAPI.Log.Write("Saved default configuration for Discord Webhook Info Mod");
                }
                catch (IOException ex)
                {
                    ModAPI.Log.Write("Failed to save default configuration for Discord Webhook Info Mod");
                    ModAPI.Log.Write($"Error: {ex.Message}");
                }
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(_configPath + _configFile);

                XmlElement root = doc.DocumentElement;
                foreach (XmlNode node in root.SelectNodes("/Configuration/*"))
                {
                    if (node.Attributes["Name"] != null && node.Attributes["Value"] != null)
                    {
                        _config.Add(node.Attributes["Name"].Value.ToLower(), node.Attributes["Value"].Value);
                    }
                }
                LastUpdate = File.GetLastWriteTime(_configPath + _configFile);
            }
            catch (Exception ex)
            {
                ModAPI.Log.Write("Error on load Config file.");
                ModAPI.Log.Write("Error: " + ex.Message);
            }
        }

        public string getString(string key)
        {
            if (LastUpdate < File.GetLastWriteTime(_configPath + _configFile))
                Load();

            if (!_config.ContainsKey(key))
                return "null";
            return _config[key];
        }

        public int getInt(string key)
        {
            if (LastUpdate < File.GetLastWriteTime(_configPath + _configFile))
                Load();

            if (!_config.ContainsKey(key))
                return -1;

            int back = -1;
            if (!int.TryParse(_config[key], out back))
                return -1;

            if (back < 0)
                return -1;
            return back;
        }

        public bool getBool(string key)
        {
            if (LastUpdate < File.GetLastWriteTime(_configPath + _configFile))
                Load();

            if (!_config.ContainsKey(key))
                return false;

            bool back;
            if (!bool.TryParse(_config[key], out back))
                return false;

            return back;
        }
    }
}
