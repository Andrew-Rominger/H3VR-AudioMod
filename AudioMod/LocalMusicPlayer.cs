using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FistVR;
using UnityEngine;
using Random = System.Random;

namespace AudioMod
{
    /// <summary>
    /// Music player that supports local MP3s
    /// </summary>
    public class LocalMusicPlayer : IMusicPlayer
    {
        private static readonly Random Rand = new Random();
        private readonly MonoBehaviour _manager;
        private AudioClip[] _takeMusic;
        private AudioClip[] _holdMusic;
        private AudioCrossFade _audioCrossFade;
        public LocalMusicPlayer(MonoBehaviour manager)
        {
            _manager = manager;
            _audioCrossFade = _manager.gameObject.GetComponent<AudioCrossFade>();
            LoadMusic();
        }

        /// <inheritdoc />
        public void PlayTakeMusic()
        {
            //Get audio crossfader and random audio clip
            var musicSource = _manager.gameObject.GetComponent<AudioCrossFade>();
            var toPlay = GetRandomAudioClip(_takeMusic);
            //Check if the clip has a start time offset
            CrossFadeToClip(musicSource, toPlay, ModConfig.Instance.ConfigFile.TakeMusicFadeTime);
        }

        /// <inheritdoc />
        public void PlayHoldMusic()
        {
            var musicSource = _manager.gameObject.GetComponent<AudioCrossFade>();

            var toPlay = GetRandomAudioClip(_holdMusic);
            CrossFadeToClip(musicSource, toPlay, ModConfig.Instance.ConfigFile.HoldMusicFadeTime);
        }

        /// <inheritdoc />
        public void SkipTakeMusic()
        {
            //Get audio crossfader
            var musicSource = _manager.gameObject.GetComponent<AudioCrossFade>();
           
            AudioClip toPlay;
            //If there are multiple songs available we need to pick a random one
            if (_takeMusic.Length > 1)
            {
                //If there is a currently playing song, we need to make sure we don't select it when picking a new one to skip to
                if (musicSource.IsPlaying)
                {
                    toPlay = GetRandomAudioClip(_takeMusic.Where(mc => mc.name != musicSource.CurrentSource.clip.name));
                }
                else
                {
                    toPlay = GetRandomAudioClip(_takeMusic);
                }
            }
            else
            {
                toPlay = _takeMusic.First();
            }

            CrossFadeToClip(musicSource, toPlay, ModConfig.Instance.ConfigFile.TakeMusicFadeTime);
        }

        /// <inheritdoc />
        public void SkipHoldMusic()
        {
           
            var musicSource = _manager.gameObject.GetComponent<AudioCrossFade>();
            AudioClip toPlay;
            if (_holdMusic.Length > 1)
            {
                if (musicSource.IsPlaying)
                {
                    toPlay = GetRandomAudioClip(_holdMusic.Where(mc => mc.name != musicSource.CurrentSource.clip.name));
                }
                else
                {
                    toPlay = GetRandomAudioClip(_holdMusic);
                }
            }
            else
            {
                toPlay = _holdMusic.First();
            }
            CrossFadeToClip(musicSource, toPlay, ModConfig.Instance.ConfigFile.HoldMusicFadeTime);
        }

        /// <inheritdoc />
        public void VolumeUp()
        {
            var musicSource = _manager.gameObject.GetComponent<AudioCrossFade>();
            musicSource.VolumeUp();
            if (ModConfig.Instance.ConfigFile.Volume <= (1.0f- AudioCrossFade.VolumeChangeAmount))
                ModConfig.Instance.ConfigFile.Volume += AudioCrossFade.VolumeChangeAmount;
            else if (ModConfig.Instance.ConfigFile.Volume > (1.0f - AudioCrossFade.VolumeChangeAmount) && ModConfig.Instance.ConfigFile.Volume < 1f)
                ModConfig.Instance.ConfigFile.Volume = 1f;
        }

        /// <inheritdoc />
        public void VolumeDown()
        {
            var musicSource = _manager.gameObject.GetComponent<AudioCrossFade>();
            musicSource.VolumeDown();
            if (ModConfig.Instance.ConfigFile.Volume >= AudioCrossFade.VolumeChangeAmount)
                ModConfig.Instance.ConfigFile.Volume -= AudioCrossFade.VolumeChangeAmount;
            else if (ModConfig.Instance.ConfigFile.Volume > 0f && ModConfig.Instance.ConfigFile.Volume < AudioCrossFade.VolumeChangeAmount)
                ModConfig.Instance.ConfigFile.Volume = 0;
        }

        private void CrossFadeToClip(AudioCrossFade musicSource, AudioClip toPlay, float fadeTime)
        {
            var name = toPlay.name.Split('.')[0];
            if (ModConfig.Instance.MusicOffsets?.ContainsKey(name) ?? false)
            {
                musicSource.CrossFade(toPlay, ModConfig.Instance.ConfigFile.Volume, fadeTime, 0, ModConfig.Instance.MusicOffsets[name]);
            }
            else
            {
                musicSource.CrossFade(toPlay, ModConfig.Instance.ConfigFile.Volume, 0);
            }
        }

        /// <summary>
        /// Loads the mp3s into AudioClips and stores them in arrays
        /// </summary>
        private void LoadMusic()
        {
            var holdMusicFolder = new DirectoryInfo("Mods\\AudioMod\\HoldMusic\\");
            var takeMusicFolder = new DirectoryInfo("Mods\\AudioMod\\TakeMusic\\");
            var holdMusicFiles = holdMusicFolder.GetFiles();
            var takeMusicFiles = takeMusicFolder.GetFiles();
            _takeMusic = new AudioClip[takeMusicFiles.Length];
            _holdMusic = new AudioClip[holdMusicFiles.Length];

            //TODO Load songs when needed?
            //TODO Load songs on background thread?
            for (var i = 0; i < holdMusicFiles.Length; i++)
            {
                //Every song needs its own importer for now, trying to reuse the same importer causes previously loaded AudioClips to be destroyed
                var importer = _manager.gameObject.AddComponent<BassImporter>();
                _holdMusic[i] = importer.ImportFile(holdMusicFiles[i].FullName);
            }

            for (var i = 0; i < takeMusicFiles.Length; i++)
            {
                var importer = _manager.gameObject.AddComponent<BassImporter>();
                _takeMusic[i] = importer.ImportFile(takeMusicFiles[i].FullName);
            }
        }

        /// <summary>
        /// Selects a random AudioClip object from the passed enumerable
        /// </summary>
        /// <param name="clips">The collection of clips to select from</param>
        /// <returns>A random audio clip from the passed collection</returns>
        private AudioClip GetRandomAudioClip(IEnumerable<AudioClip> clips)
        {
            var audioClips = clips.ToList();
            if (audioClips.Count > 1)
            {
                var audioClipList = audioClips.Where(c => c != _audioCrossFade.CurrentSource.clip).ToList();
                return audioClipList[Rand.Next(0, audioClipList.ToList().Count)];
            }
            return audioClips.First();
        }
    }
}
