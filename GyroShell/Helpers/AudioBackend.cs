using CoreAudio;
using System;

namespace GyroShell.Helpers
{
    public class AudioBackend
    {
        public static MMDeviceEnumerator devEnum = new MMDeviceEnumerator(Guid.Empty);
        public static MMDevice audioDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    }
}
