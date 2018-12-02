using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioMod
{
    public interface IMusicPlayer
    {
        /// <summary>
        /// Starts playing a new Take song
        /// </summary>
        void PlayTakeMusic();
        /// <summary>
        /// Starts playing a new Hold song
        /// </summary>
        void PlayHoldMusic();
        /// <summary>
        /// Skips the currently playing Take song
        /// </summary>
        void SkipTakeMusic();
        /// <summary>
        /// Skips the currently playing Hold song
        /// </summary>
        void SkipHoldMusic();
        /// <summary>
        /// Increase the volume of the currently playing song
        /// </summary>
        void VolumeUp();
        /// <summary>
        /// Decreases the volume of the currently playing song
        /// </summary>
        void VolumeDown();
        
    }
}
