using System;
using System.Collections.Generic;
using System.IO;

namespace AudioMod
{
    [Serializable]
    public class ConfigFile
    {
        public float Volume;
        public float HoldMusicFadeTime;
        public float TakeMusicFadeTime;

        public override string ToString()
        {
            var str = $"Volume: {Volume}\nHoldMusicFadeTime: {HoldMusicFadeTime}\nTakeMusicFadeTime: {TakeMusicFadeTime}";
            return str;
        }
    }

    public class ModConfig
    {
        public ConfigFile ConfigFile { get; set; }
        public Dictionary<string, int> MusicOffsets { get; set; } = new Dictionary<string, int>();

        public void LoadMusicOffsets(FileInfo fileInfo)
        {
            foreach (var line in File.ReadAllLines(fileInfo.FullName))
            {
                var songName = line.Split(null)[0];
                var amountStr = line.Split(null)[1];
                var min = Convert.ToInt32(amountStr.Split(':')[0]);
                var sec = Convert.ToInt32(amountStr.Split(':')[1]);
                MusicOffsets.Add(songName, (min * 60) + sec);
            }
        }
    }
}
