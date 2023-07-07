using System;
using CoreAudio;

namespace GyroShell.Helpers
{
   internal class AudioBackend
   {
        internal static MMDeviceEnumerator devEnum = new MMDeviceEnumerator(Guid.Empty);
        internal static MMDevice audioDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
   } 
}
