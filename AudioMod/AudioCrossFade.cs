using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace AudioMod
{
    //Credit Igor Aherne. Feel free to use as you wish, but mention me in credits :)
    //www.facebook.com/igor.aherne

    //Modified by Andrew Rominger

    /// <summary>
    /// Smoothly transitions between AudioClips
    /// </summary>
    public class AudioCrossFade : MonoBehaviour
    {
        public AudioSource Source0;
        public AudioSource Source1;
        public bool IsSongOver;
        #region internal vars

        private bool _source0Playing = true; //is _source0 currently the active AudioSource (plays some sound right now)

        private Coroutine _zerothSourceFadeRoutine = null;
        private Coroutine _firstSourceFadeRoutine = null;

        private float _currentSourcePlayLength;
        #endregion

        #region internal functionality

        void Reset()
        {
            Update();
        }


        void Awake()
        {
            Update();
        }


        void Update()
        {
            
            //constantly check if our game object doesn't contain audio sources which we are referencing.
            //if the _source0 or _source1 contain obsolete references (most likely 'null'), then we will re-init them:
            if (Source0 == null || Source1 == null)
            {
                //re-connect _soruce0 and _source1 to the ones in attachedSources[]
                var attachedSources = gameObject.GetComponents(typeof(AudioSource));

                //For some reason, unity doesn't accept "as AudioSource[]" casting. We would get
                //'null' array instead if we would attempt. Need to re-create a new array:
                var sources = attachedSources.Select(c => c as AudioSource).ToArray();

                InitSources(sources);
            }
            else if(!IsSongOver)
            {
                _currentSourcePlayLength += Time.deltaTime;
                if (_currentSourcePlayLength >= CurrentSource.clip.length)
                {
                    _currentSourcePlayLength = 0f;
                    IsSongOver = true;
                }
            }
        }

        //re-establishes references to audio sources on this game object:
        private void InitSources(IList<AudioSource> audioSources)
        {
            if (ReferenceEquals(audioSources, null) || audioSources.Count == 0)
            {
                Source0 = gameObject.AddComponent<AudioSource>();
                Source1 = gameObject.AddComponent<AudioSource>();
                return;
            }

            switch (audioSources.Count)
            {
                case 1:
                    Source0 = audioSources[0];
                    Source1 = gameObject.AddComponent<AudioSource>();
                    break;
                default:
                    Source0 = audioSources[0];
                    Source1 = audioSources[1];
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Gradually shifts the current sound to a new AudioClip
        /// </summary>
        /// <param name="clipToPlay">AudioClip to start playing</param>
        /// <param name="maxVolume">Max volume to fade to. range: 0(mute) to 1(max) </param>
        /// <param name="fadingTime">How long to fade between clips. Use 0 the change instantly</param>
        /// <param name="delayBeforeCrossFade">Time to wait before beginning fade</param>
        /// <param name="timeSkip">Number of seconds to skip in the incoming sound.</param>
        public void CrossFade(AudioClip clipToPlay, float maxVolume, float fadingTime, float delayBeforeCrossFade = 0, float timeSkip = 0)
        {
            IsSongOver = false;
            _currentSourcePlayLength = 0f;
            StartCoroutine(Fade(clipToPlay, maxVolume, fadingTime, delayBeforeCrossFade, timeSkip));
        }

        /// <summary>
        /// Decreases the currently playing AudioSource's volume by .1
        /// </summary>
        public void VolumeDown()
        {
            if (CurrentSource.volume >= .01f)
                CurrentSource.volume -= .01f;
            else if (CurrentSource.volume > 0f && CurrentSource.volume < .01f)
                CurrentSource.volume = 0f;
        }

        /// <summary>
        /// Increases the currently playing AudioSource's volume by .1. Wont do anything if volume is already 1
        /// </summary>
        public void VolumeUp()
        {
            if (CurrentSource.volume <= .99f)
                CurrentSource.volume += .01f;
            else if (CurrentSource.volume > .99f && CurrentSource.volume < 1f)
                CurrentSource.volume = 1f;
        }

        /// <summary>
        /// The currently playing audio source
        /// </summary>
        public AudioSource CurrentSource => _source0Playing ? Source0 : Source1;

        private IEnumerator Fade(AudioClip playMe, float maxVolume, float fadingTime, float delayBeforeCrossFade = 0, float timeSkip = 0)
        {
            if (delayBeforeCrossFade > 0)
            {
                yield return new WaitForSeconds(delayBeforeCrossFade);
            }

            if (_source0Playing)
            {
                // _source0 is currently playing the most recent AudioClip so launch on source1
                Source1.clip = playMe;
                Source1.time = timeSkip;
                Source1.Play();
                Source1.volume = 0;

                //Stop if currently fading
                if (_firstSourceFadeRoutine != null)
                {
                    StopCoroutine(_firstSourceFadeRoutine);
                }

                //Fade Source1 up to maxVolume
                _firstSourceFadeRoutine = StartCoroutine(fadeSource(Source1, Source1.volume ,maxVolume, fadingTime));

                if (_zerothSourceFadeRoutine != null)
                {
                    StopCoroutine(_zerothSourceFadeRoutine);
                }

                //Fade Source0 down to 0
                _zerothSourceFadeRoutine = StartCoroutine(fadeSource(Source0,Source0.volume,0, fadingTime));

                _source0Playing = false;

                yield break;
            }

            //otherwise, Source1 is currently active, so play on Source0
            Source0.clip = playMe;
            Source0.time = timeSkip;
            Source0.Play();
            
            Source0.volume = 0;

            if (_zerothSourceFadeRoutine != null)
            {
                StopCoroutine(_zerothSourceFadeRoutine);
            }
            _zerothSourceFadeRoutine = StartCoroutine(fadeSource(Source0, Source0.volume, maxVolume, fadingTime));

            if (_firstSourceFadeRoutine != null)
            {
                StopCoroutine(_firstSourceFadeRoutine);
            }
            _firstSourceFadeRoutine = StartCoroutine(fadeSource(Source1, Source1.volume, 0, fadingTime));
            _source0Playing = true;
        }


        private IEnumerator fadeSource(AudioSource sourceToFade, float startVolume, float endVolume, float duration)
        {
            var startTime = Time.time;

            while (true)
            {
                if (duration == 0)
                {
                    sourceToFade.volume = endVolume;
                    break;//break, to prevent division by  zero
                }
                var elapsed = Time.time - startTime;

                sourceToFade.volume = Mathf.Clamp01(Mathf.Lerp(startVolume, endVolume, elapsed / duration));

                if (sourceToFade.volume == endVolume)
                {
                    break;
                }
                yield return null;
            }
        }

        /// <summary>
        /// Are either sources currently playing
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                if (Source0 == null || Source1 == null)
                {
                    return false;
                }
                
                return Source0.isPlaying || Source1.isPlaying;
            }
        }
    }
}