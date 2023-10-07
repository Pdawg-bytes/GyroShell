using System;

namespace GyroShell.Library.Services.Hardware
{
    /// <summary>
    /// Defines a platform-agnostic service interface to get sound data.
    /// </summary>
    public interface ISoundService
    {
        /// <summary>
        /// The current volume.
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// Checks if the audio is muted.
        /// </summary>
        public bool IsMuted { get; set; }

        /// <summary>
        /// An event raised when the volume is changed, including
        /// when it gets muted.
        /// </summary>
        public event EventHandler OnVolumeChanged;
    }
}
