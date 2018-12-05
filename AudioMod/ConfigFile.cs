using System;
using System.Collections.Generic;
using System.IO;
using AudioMod;
using UnityEngine;

namespace AudioMod
{
    [Serializable]
    public class ConfigFile : ICloneable
    {
        private static ConfigFile _instance;
        public static ConfigFile Default = new ConfigFile()
        {
            HoldMusicFadeTime = 3.0f,
            HoldPlaylist = "a",
            MeatmasSongName = "",
            MusicSource = "local",
            TakeMusicFadeTime = 3.0f,
            TakePlaylist = "a",
            Volume = .5f
        };
        public static ConfigFile Instance
        {
            get
            {
                if (_instance == null)
                {
                    var configFile = new FileInfo("Mods\\AudioMod\\AudioModConfig.json");
                    if (!configFile.Exists)
                        File.WriteAllText(configFile.FullName, JsonUtility.ToJson(Default));
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

        public object Clone()
        {
            return new ConfigFile()
            {
                HoldMusicFadeTime = this.HoldMusicFadeTime,
                HoldPlaylist = this.HoldPlaylist,
                MeatmasSongName = this.MeatmasSongName,
                MusicSource =  this.MusicSource,
                TakeMusicFadeTime =  this.TakeMusicFadeTime,
                TakePlaylist =  this.TakePlaylist,
                Volume = this.Volume
            };
        }
    }
}
public class ModConfig : ICloneable
{
    private static ModConfig _instance;
    public static ModConfig Default = new ModConfig()
    {
        ConfigFile = (ConfigFile)ConfigFile.Default.Clone(),
        MusicOffsets = new Dictionary<string, int>()
    };
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
        if (fileInfo.Exists)
        {
            foreach (var line in File.ReadAllLines(fileInfo.FullName))
            {
                var lineComp = line.Split(null);
                MusicOffsets.Add(lineComp[0], (Convert.ToInt32(lineComp[1].Split(':')[0]) * 60) + Convert.ToInt32(lineComp[1].Split(':')[1]));
            }
        }
        else
        {
            File.Create(fileInfo.FullName);
        }
        
    }

    public object Clone()
    {
        var newConfig = new ModConfig
        {
            ConfigFile = (ConfigFile)ConfigFile.Clone(),
            MusicOffsets = new Dictionary<string, int>()

        };
        foreach (var offset in this.MusicOffsets)
        {
            newConfig.MusicOffsets.Add(offset.Key, offset.Value);
        }

        return newConfig;
    }
}