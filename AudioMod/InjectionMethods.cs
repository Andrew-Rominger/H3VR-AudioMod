using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FistVR;
using Kolibri.Lib;
using Mono.Cecil.Inject;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AudioMod
{
    /// <summary>
    /// Holds all the method calls to be injected into the game
    /// </summary>
    public class InjectionMethods
    {
        public static readonly ManagerWrapper CurrentManagerWrapper = new ManagerWrapper();

        /// <summary>
        /// Spawns the MusicController object in the TAH scene
        /// </summary>
        /// <param name="manager">The manager for the game</param>
        [InjectMethod(typeof(TNH_Manager), "InitBeginningEquipment", MethodInjectionInfo.MethodInjectionLocation.Bottom, InjectFlags.PassInvokingInstance)]
        public static void AddMusicControllerTNH(TNH_Manager manager)
        {
            try
            {
                var gm = new GameObject();
                var mci = gm.AddComponent<MusicController>();
                var musicController = Object.Instantiate(mci.GameObject, GM.CurrentPlayerBody.Head.transform.position, GM.CurrentPlayerBody.Head.transform.rotation);
                manager.AddObjectToTrackedList(musicController);
                //Destroy the original gameobject
                Object.Destroy(gm);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Application.Quit();
            }

        }

        /// <summary>
        /// Spawns the MusicController object in the TAH scene
        /// </summary>
        /// <param name="manager">The manager for the game</param>
        [InjectMethod(typeof(TAH_Manager), nameof(TAH_Manager.BeginGame), MethodInjectionInfo.MethodInjectionLocation.Bottom, InjectFlags.PassInvokingInstance)]
        public static void AddMusicControllerOldTAH(TAH_Manager manager)
        {
            try
            {
                var gm = new GameObject();
                var mci = gm.AddComponent<MusicController>();
                var musicController = Object.Instantiate(mci.GameObject, GM.CurrentPlayerBody.Head.transform.position, GM.CurrentPlayerBody.Head.transform.rotation);
                manager.AddObjectToTrackedList(musicController);
                //Destroy the original gameobject
                Object.Destroy(gm);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Application.Quit();
            }

        }

        /// <summary>
        /// Changes the music playing during the Meatmas scene to one specified by the config file
        /// </summary>
        /// <param name="manager"></param>
        [InjectMethod(typeof(HG_GameManager), nameof(HG_GameManager.StartMusic), MethodInjectionInfo.MethodInjectionLocation.Bottom, InjectFlags.PassInvokingInstance)]
        public static void MeatmasMusic(HG_GameManager manager)
        {
            if (!String.IsNullOrEmpty(ConfigFile.Instance.MeatmasSongName))
            {
                Assembly.Load("System.Windows.Forms");
                //Create importer for song
                var musicLoader = manager.gameObject.AddComponent<BassImporter>();
                //Create a list of all the music available in TakeMusic and HoldMusic
                var music = new List<FileInfo>();
                music.AddRange(new DirectoryInfo("Mods\\AudioMod\\TakeMusic\\").GetFiles());
                music.AddRange(new DirectoryInfo("Mods\\AudioMod\\HoldMusic\\").GetFiles());
                var toPlay = music.First(mf => mf.Name == ConfigFile.Instance.MeatmasSongName);
                manager.AudSource_BattleMusic.clip = musicLoader.ImportFile(toPlay.FullName);
                //Set the BattleMusic clip
            }
        }

        /// <summary>
        /// Injected into the top of TAH_Manager.BeginGame, used to load music, silence FMOD music, and add components for later use
        /// </summary>
        /// <param name="manager">The manager we are injecting into</param>
        [InjectMethod(typeof(TAH_Manager), nameof(TAH_Manager.BeginGame), injectFlags: InjectFlags.PassInvokingInstance)]
        public static void BeginGameInject(TAH_Manager manager)
        {
            try
            {
                CurrentManagerWrapper.CurrentManager = manager;
                //This is needed for BASS
                Assembly.Load("System.Windows.Forms");

                //Add the needed components
                var audioMod = manager.gameObject.AddComponent<AudioModComponent>();
                manager.gameObject.AddComponent<AudioCrossFade>();

                audioMod.Manager = manager;

                audioMod.Init();
                audioMod.SilenceDefaultMusic();
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Application.Quit();
            }
        }
        
        /// <summary>
        /// Injected into the top of TAH_Manager.BeginGame, used to load music, silence FMOD music, and add components for later use
        /// </summary>
        /// <param name="manager">The manager we are injecting into</param>
        [InjectMethod(typeof(TNH_Manager), "InitBeginningEquipment", MethodInjectionInfo.MethodInjectionLocation.Bottom, InjectFlags.PassInvokingInstance)]
        public static void BeginGameInjectNewTakeAndHold(TNH_Manager manager)
        {
            try
            {
                CurrentManagerWrapper.CurrentManager = manager;
                //This is needed for BASS
                Assembly.Load("System.Windows.Forms");

                //Add the needed components
                var audioMod = manager.gameObject.AddComponent<AudioModComponent>();
                manager.gameObject.AddComponent<AudioCrossFade>();

                audioMod.Manager = manager;

                audioMod.Init();
                audioMod.SilenceDefaultMusic();
            }
            catch (Exception e)
            {
                Logger.Log(e);
                Application.Quit();
            }
        }

        /// <summary>
        /// Injected into FMODController.SwitchTo to start playing appropriate music
        /// </summary>
        /// <param name="musicIndex">Type of switch. See FVRFMODController.SwitchTo</param>
        [InjectMethod(typeof(FVRFMODController), "SwitchTo", injectFlags: InjectFlags.PassParametersVal)]
        public static void StartCustomMusic(int musicIndex, float timeDelayStart, bool shouldStop, bool shouldDeadStop)
        {
            try
            {
                var audioModComponent = CurrentManagerWrapper.CurrentManager.GetComponent<AudioModComponent>();
                
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
    }
}
