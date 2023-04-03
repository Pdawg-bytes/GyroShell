using System;
using CoreAudio;

namespace GyroShell.Helpers
{
   public class AudioBackend
   {
        public static MMDeviceEnumerator devEnum = new MMDeviceEnumerator(Guid.Empty);
        public static MMDevice audioDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
   } 
}
