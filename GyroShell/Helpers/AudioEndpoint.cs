using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Helpers
{
    public class AudioEndpoint
    {
        private static IAudioEndpointVolume GetDefaultAudioEndpointVolume()
        {
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            IMMDevice speakers;
            const int eRender = 0;
            const int eMultimedia = 1;

            deviceEnumerator.GetDefaultAudioEndpoint(eRender, eMultimedia, out speakers);

            Guid iidIAudioEndpointVolume = typeof(IAudioEndpointVolume).GUID;
            IAudioEndpointVolume endpointVolume;
            speakers.Activate(ref iidIAudioEndpointVolume, 0, IntPtr.Zero, out endpointVolume);

            return endpointVolume;
        }
    }

    [ComImport]
    [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
    public class MMDeviceEnumerator
    {
    }

    [ComImport]
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDeviceEnumerator
    {
        void NotImpl1();
        void NotImpl2();
        void NotImpl3();
        void GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice ppDevice);
    }

    [ComImport]
    [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDevice
    {
        void Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
    }

    [ComImport]
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioEndpointVolumeCallback
    {
        void OnNotify(IntPtr notifyData);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AUDIO_VOLUME_NOTIFICATION_DATA
    {
        public Guid guidEventContext;
        public bool bMuted;
        public float fMasterVolume;
        public uint nChannels;
        public float afChannelVolumes;
    }

    public class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback
    {
        public void OnNotify(IntPtr notifyData)
        {
            var data = (AUDIO_VOLUME_NOTIFICATION_DATA)Marshal.PtrToStructure(notifyData, typeof(AUDIO_VOLUME_NOTIFICATION_DATA));

            Console.WriteLine($"Volume change detected: Master volume is {data.fMasterVolume}, mute status is {data.bMuted}");
        }
    }

    [ComImport]
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioEndpointVolume
    {
        void NotImpl1();
        void NotImpl2();
        void NotImpl3();
        void NotImpl4();
        void NotImpl5();
        void NotImpl6();
        void NotImpl7();
        void NotImpl8();
        void NotImpl9();
        void NotImpl10();
        void SetMasterVolumeLevelScalar(float fLevel, ref Guid pguidEventContext);
        void NotImpl11();
        void NotImpl12();
        void NotImpl13();
        void NotImpl14();
        void NotImpl15();
        void NotImpl16();
        void NotImpl17();
        void NotImpl18();
        void NotImpl19();
        void RegisterControlChangeNotify(IAudioEndpointVolumeCallback pNotify);
        void UnregisterControlChangeNotify(IAudioEndpointVolumeCallback pNotify);
    }
}
