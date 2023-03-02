using CoreAudio;
using System;

namespace GyroShell.Helpers
{
    public class AudioBackend
    {
        public enum VolumeUnit
        {
            Decibel,
            Scalar
        };
        /*public static float GetSystemVolume(VolumeUnit vUnit)
        {
            Guid guid = typeof(MMDeviceEnumerator).GUID;
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator(guid);
            MMDevice defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(CoreAudio.EDataFlow.eRender, CoreAudio.ERole.eMultimedia);

            float currentVolume = 0;

            if (vUnit == VolumeUnit.Decibel)
            {
                currentVolume = (float)defaultDevice.AudioEndpointVolume.MasterVolumeLevel;
            }
            else if (vUnit == VolumeUnit.Scalar)
            {
                currentVolume = defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
            }

            return currentVolume;
        }*/
    }
}
