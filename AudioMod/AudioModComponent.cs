using FistVR;
using FMOD.Studio;
using UnityEngine;

namespace AudioMod
{
    /// <summary>
    /// Component which manages song playback 
    /// </summary>
    public class AudioModComponent : MonoBehaviour
    {
        public MonoBehaviour Manager { get; set; }
        public IMusicPlayer MusicPlayer { get; private set; }
        private AudioCrossFade _audioPlayer;
        public void Init()
        {
            MusicPlayer = new LocalMusicPlayer(Manager);
            _audioPlayer = Manager.gameObject.GetComponent<AudioCrossFade>();
        }

        void Update()
        {
            if (_audioPlayer.IsSongOver)
            {
                SongEnd();
            }
        }

        /// <summary>
        /// Called by the CrossFader when the current song ends
        /// </summary>
        public void SongEnd()
        {
            if (InjectionMethods.CurrentManagerWrapper.GetState() == ManagerWrapper.State.Taking)
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
            Manager.GetComponent<FVRFMODController>().GetField<Bus>("MasterBus").setMute(true);
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
