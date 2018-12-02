using FistVR;
using FMOD.Studio;
using UnityEngine;
using Random = System.Random;

namespace AudioMod
{
    /// <summary>
    /// Component which manages song playback 
    /// </summary>
    public class AudioModComponent : MonoBehaviour
    {
        public TAH_Manager Manager { get; set; }
        public IMusicPlayer MusicPlayer { get; private set; }

        public void Init()
        {
            MusicPlayer = new LocalMusicPlayer(Manager);
        }
        /// <summary>
        /// Called by the CrossFader when the current song ends
        /// </summary>
        public void SongEnd()
        {
            if (GM.TAHMaster.State == TAH_Manager.TAHGameState.Taking)
                PlayTakeMusic();
            else
                PlayHoldMusic();
        }
        /// <summary>
        /// Stops the game's default music from playing
        /// </summary>
        public void SilenceDefaultMusic()
        {
            //use reflection to get the FMODControllers bus and mute it
            Manager.FMODController.GetField<Bus>("MasterBus").setMute(true);
        }

        public void PlayHoldMusic()
        {
            MusicPlayer.PlayHoldMusic();
        }

        public void SkipHoldMusic()
        {
            MusicPlayer.SkipHoldMusic();
        }

        public void IncreaseMusicVolume()
        {
            MusicPlayer.VolumeUp();
        }
        public void DecreaseMusicVolume()
        {
            MusicPlayer.VolumeDown();
        }

        public void PlayTakeMusic()
        {
            MusicPlayer.PlayTakeMusic();
        }
        public void SkipTakeMusic()
        {
            MusicPlayer.SkipTakeMusic();
        }
    }
}
