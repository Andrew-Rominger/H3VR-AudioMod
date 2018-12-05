using System;
using System.IO;
using Un4seen.Bass;
using UnityEngine;

namespace AudioMod
{
    public class BassImporter : MonoBehaviour
    {
        private AudioClip _audioClip;
        private int sample;
        

        /// <summary>
        /// Imports an mp3 file. Only the start of a file is actually imported.
        /// The remaining part of the file will be imported bit by bit to speed things up. 
        /// </summary>
        /// <returns>AudioClip containing the song. </returns>
        /// <param name='filePath'>Path to mp3 file.</param>
        public AudioClip ImportFile(string filePath)
        {
            //If reusing importer destroy old AudioClip
            if (_audioClip != null)
                AudioClip.Destroy(_audioClip);
            
            //Try and init BASS
            if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                //Load the file and get information about it
                sample = Bass.BASS_SampleLoad(filePath, 0, 0, 1, BASSFlag.BASS_SAMPLE_FLOAT);
                var info = Bass.BASS_SampleGetInfo(sample);
                var lengthSamples = (int)(info.length / sizeof(float));
                //Create a new AudioClip object
                _audioClip = AudioClip.Create(Path.GetFileNameWithoutExtension(filePath), lengthSamples / info.chans, info.chans, info.freq, false);
                //Load the song data
                var data = new float[lengthSamples];
                Bass.BASS_SampleGetData(sample, data);
                //Set the audio clip to the song
                _audioClip.SetData(data, 0);
                //Free up BASS 
                Bass.BASS_SampleFree(sample);
                Bass.BASS_Free();
            }
            else
            {
                Logger.Log("Bass init failed");
                Logger.Log(Bass.BASS_ErrorGetCode().ToString());
                Application.Quit();
            }

            _audioClip.name = Path.GetFileName(filePath);
            return _audioClip;
        }
    }
}
