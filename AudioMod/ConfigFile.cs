using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AudioMod
{
    [Serializable]
    public class ConfigFile
    {
        private static ConfigFile _instance;

        public static ConfigFile Instance
        {
            get
            {
                if (_instance == null)
                {
                    var configFile = new FileInfo("Mods\\AudioMod\\AudioModConfig.json");
                    var configFileText = File.ReadAllText(configFile.FullName);
                    _instance = JsonUtility.FromJson<ConfigFile>(configFileText);
                }

                return _instance;
            }
        }

        public float Volume;
        public float HoldMusicFadeTime;
        public float TakeMusicFadeTime;
        public string MeatmasSongName;
        //Will be used later when multiple sources are supported
        public string MusicSource;
        public string TakePlaylist;
        public string HoldPlaylist;
        public override string ToString()
        {
            return $"Volume: {Volume}\nHoldMusicFadeTime: {HoldMusicFadeTime}\nTakeMusicFadeTime: {TakeMusicFadeTime}";
        }
    }

    public class ModConfig
    {
        private static ModConfig _instance;

        public static ModConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ModConfig()
                    {
                        ConfigFile = ConfigFile.Instance,
                        MusicOffsets = new Dictionary<string, int>()
                    };
                    _instance.LoadMusicOffsets(new FileInfo("Mods\\AudioMod\\MusicOffsets.txt"));
                }
                return _instance;
            }
        }

        public ConfigFile ConfigFile { get; set; }
        public Dictionary<string, int> MusicOffsets { get; set; } = new Dictionary<string, int>();

        public void LoadMusicOffsets(FileInfo fileInfo)
        {
            foreach (var line in File.ReadAllLines(fileInfo.FullName))
            {
                var lineComp = line.Split(null);
                MusicOffsets.Add(lineComp[0], (Convert.ToInt32(lineComp[1].Split(':')[0]) * 60) + Convert.ToInt32(lineComp[1].Split(':')[1]));
            }
        }
    }
}
