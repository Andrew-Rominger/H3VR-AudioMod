using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FistVR;
using FMOD.Studio;
using Kolibri.Lib;
using Mono.Cecil.Inject;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace AudioMod
{
    public class AudioMod
    {
        [InjectMethod(typeof(TAH_Manager), nameof(TAH_Manager.BeginGame),
            MethodInjectionInfo.MethodInjectionLocation.Bottom, InjectFlags.PassInvokingInstance)]
        public static void AddMusicController(TAH_Manager manager)
        {
            try
            {
                var gm = new GameObject();
                var mci = gm.AddComponent<MusicController>();
                var musicController = Object.Instantiate(mci.GameObject, GM.CurrentPlayerBody.Head.transform.position, GM.CurrentPlayerBody.Head.transform.rotation);
                manager.AddObjectToTrackedList(musicController);
                
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Application.Quit();
            }

        }

        private static Random rand = new Random();
        [InjectMethod(typeof(FistVR.TAH_Manager), "BeginGame", injectFlags:InjectFlags.PassInvokingInstance)]
        public static void BeginGameInject(TAH_Manager manager)
        {
            try
            {
                Assembly.Load("System.Windows.Forms");
                
                var audioMod = manager.gameObject.AddComponent<AudioModComponent>();
                manager.gameObject.AddComponent<DoubleAudioSource>();


                var configFile = new FileInfo("Mods\\AudioMod\\AudioModConfig.json");
                var configFileText = File.ReadAllText(configFile.FullName);

                File.WriteAllText("ConfigFileContents.txt", configFileText);
                var config = JsonUtility.FromJson<ConfigFile>(configFileText);
                File.WriteAllText("Log.txt", $"{config == null}");
                File.WriteAllText("Log.txt", "Test");
                audioMod.Config = new ModConfig
                {
                    ConfigFile = config
                };
                audioMod.Config.LoadMusicOffsets(new FileInfo("Mods\\AudioMod\\MusicOffsets.txt"));
                audioMod.Manager = manager;

                audioMod.LoadMusic();
                audioMod.SilenceDefaultMusic();
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Application.Quit();
            }
        }

        [InjectMethod(typeof(FVRFMODController), "SwitchTo", injectFlags:InjectFlags.PassParametersVal)]
        public static void StartCustomMusic(int musicIndex, float timeDelayStart, bool shouldStop, bool shouldDeadStop)
        {
            try
            {
                var audioModComponent = GM.TAHMaster.GetComponent<AudioModComponent>();
                switch (musicIndex)
                {
                    case 1:
                        audioModComponent.PlayHoldMusic();
                        break;
                    case 0:
                        audioModComponent.PlayTakeMusic();
                        break;
                }
                audioModComponent.SilenceDefaultMusic();
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Application.Quit();
            }
        }

        


        public class AudioModComponent : MonoBehaviour
        {
            private AudioClip[] TakeMusic;
            private AudioClip[] HoldMusic;
            public ModConfig Config { get; set; }
            public TAH_Manager Manager { get; set; }

            public void SilenceDefaultMusic()
            {
                Manager.FMODController.GetField<Bus>("MasterBus").setMute(true);
            }

            public void PlayHoldMusic()
            {
                var musicSource = Manager.gameObject.GetComponent<DoubleAudioSource>();
                var holdClip = GetRandomAudioClip(HoldMusic);
                if (Config.MusicOffsets.ContainsKey(holdClip.name))
                {
                    musicSource.CrossFade(holdClip, Config.ConfigFile.Volume, Config.ConfigFile.TakeMusicFadeTime,0, Config.MusicOffsets[holdClip.name]);
                }
                else
                {
                    musicSource.CrossFade(holdClip, Config.ConfigFile.Volume, Config.ConfigFile.TakeMusicFadeTime);
                }
            }

            public void SkipHoldMusic()
            {
                var musicSource = Manager.gameObject.GetComponent<DoubleAudioSource>();
                AudioClip toPlay;
                if (HoldMusic.Length > 1)
                {
                    if (musicSource.isPlaying)
                    {
                        toPlay = GetRandomAudioClip(HoldMusic.Where(mc => mc.name != musicSource.CurrentSource.clip.name));
                    }
                    else
                    {
                        toPlay = GetRandomAudioClip(HoldMusic);
                    }
                }
                else
                {
                    toPlay = HoldMusic.First();
                }


                if (Config.MusicOffsets.ContainsKey(toPlay.name))
                {
                    musicSource.CrossFade(toPlay, Config.ConfigFile.Volume, 0, 0, Config.MusicOffsets[toPlay.name]);
                }
                else
                {
                    musicSource.CrossFade(toPlay, Config.ConfigFile.Volume, 0);
                }
            }
            public void IncreaseMusicVolume()
            {
                var musicSource = Manager.gameObject.GetComponent<DoubleAudioSource>();
                musicSource.VolumeUp();
                if (Config.ConfigFile.Volume <= .9f)
                    Config.ConfigFile.Volume += .1f;
                else if (Config.ConfigFile.Volume > .9f && Config.ConfigFile.Volume < 1f)
                    Config.ConfigFile.Volume = 1f;
            }
            public void DecreaseMusicVolume()
            {
                var musicSource = Manager.gameObject.GetComponent<DoubleAudioSource>();
                musicSource.VolumeDown();
                if (Config.ConfigFile.Volume >= .1f)
                    Config.ConfigFile.Volume -= .1f;
                else if (Config.ConfigFile.Volume > 0f && Config.ConfigFile.Volume < .1f)
                    Config.ConfigFile.Volume = 0;
            }

            public void PlayTakeMusic()
            {
                var musicSource = Manager.gameObject.GetComponent<DoubleAudioSource>();
                var toPlay = GetRandomAudioClip(TakeMusic);
               
               
                if (Config.MusicOffsets.ContainsKey(toPlay.name))
                {
                    musicSource.CrossFade(toPlay, Config.ConfigFile.Volume, Config.ConfigFile.TakeMusicFadeTime,0, Config.MusicOffsets[toPlay.name]);
                }
                else
                {
                    musicSource.CrossFade(toPlay, Config.ConfigFile.Volume, Config.ConfigFile.TakeMusicFadeTime);
                }
            }

            public void SkipTakeMusic()
            {
                var musicSource = Manager.gameObject.GetComponent<DoubleAudioSource>();
                AudioClip toPlay;
                if (TakeMusic.Length > 1)
                {
                    if (musicSource.isPlaying)
                    {
                        toPlay = GetRandomAudioClip(TakeMusic.Where(mc => mc.name != musicSource.CurrentSource.clip.name));
                    }
                    else
                    {
                        toPlay = GetRandomAudioClip(TakeMusic);
                    }
                }
                else
                {
                    toPlay = TakeMusic.First();
                }


                if (Config.MusicOffsets.ContainsKey(toPlay.name))
                {
                    musicSource.CrossFade(toPlay, Config.ConfigFile.Volume, 0, 0, Config.MusicOffsets[toPlay.name]);
                }
                else
                {
                    musicSource.CrossFade(toPlay, Config.ConfigFile.Volume, 0);
                }
            }

            private AudioClip GetRandomAudioClip(IEnumerable<AudioClip> clips)
            {
                var l = clips.ToList();
                return l[rand.Next(0, l.Count)];
            }
            

            public void LoadMusic()
            {
                var holdMusicFolder = new DirectoryInfo("Mods\\AudioMod\\HoldMusic\\");
                var takeMusicFolder = new DirectoryInfo("Mods\\AudioMod\\TakeMusic\\");
                var holdMusicFiles = holdMusicFolder.GetFiles();
                var takeMusicFiles = takeMusicFolder.GetFiles();
                TakeMusic = new AudioClip[takeMusicFiles.Length];
                HoldMusic = new AudioClip[holdMusicFiles.Length];
                for (var i = 0; i < holdMusicFiles.Length; i++)
                {
                    var importer = Manager.gameObject.AddComponent<BassImporter>();
                    HoldMusic[i] = importer.ImportFile(holdMusicFiles[i].FullName);
                }
                for (var i = 0; i < takeMusicFiles.Length; i++)
                {
                    var importer = Manager.gameObject.AddComponent<BassImporter>();
                    TakeMusic[i] = importer.ImportFile(takeMusicFiles[i].FullName);
                }
               
            }
        }
    }

    
}
